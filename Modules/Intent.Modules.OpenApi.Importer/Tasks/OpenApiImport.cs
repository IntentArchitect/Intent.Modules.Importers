using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.Engine;
using Intent.MetadataSynchronizer;
using Intent.MetadataSynchronizer.Configuration;
using Intent.Modules.Json.Importer.Tasks.Helpers;
using Intent.Modules.Json.Importer.Tasks.Models;
using Intent.Modules.OpenApi.Importer.Importer;
using Intent.Plugins;
using Intent.Utils;

namespace Intent.Modules.OpenApi.Importer.Tasks;

public class OpenApiImport : ModuleTaskBase<ImportSettings, object?>
{
    private readonly IMetadataManager _metadataManager;
    private readonly IApplicationConfigurationProvider _configurationProvider;

    public OpenApiImport(IMetadataManager metadataManager, IApplicationConfigurationProvider configurationProvider)
    {
        _metadataManager = metadataManager;
        _configurationProvider = configurationProvider;
    }

    public override string TaskTypeId => "Intent.Modules.OpenApi.Importer.Tasks.OpenApiImport";
    public override string TaskTypeName => "OpenApi Document Import";

    protected override ValidationResult ValidateInputModel(ImportSettings inputModel)
    {
        if (inputModel is null)
        {
            return ValidationResult.ErrorResult("Invalid import request.");
        }

        if (string.IsNullOrWhiteSpace(inputModel.OpenApiSpecificationFile))
        {
            return ValidationResult.ErrorResult("OpenAPI file path or URL is required.");
        }

        if (string.IsNullOrWhiteSpace(inputModel.PackageId))
        {
            return ValidationResult.ErrorResult("Package ID is required.");
        }

        if (string.IsNullOrWhiteSpace(inputModel.ServiceType))
        {
            return ValidationResult.ErrorResult("Service type is required.");
        }

        if (!Enum.TryParse<ServiceType>(inputModel.ServiceType, true, out _))
        {
            return ValidationResult.ErrorResult($"Service type '{inputModel.ServiceType}' is not valid.");
        }

        var persistenceValue = string.IsNullOrWhiteSpace(inputModel.SettingPersistence)
            ? nameof(SettingPersistence.None)
            : inputModel.SettingPersistence;
        if (!Enum.TryParse<SettingPersistence>(persistenceValue, true, out _))
        {
            return ValidationResult.ErrorResult($"Persist Settings value '{inputModel.SettingPersistence}' is not valid.");
        }

        return ValidationResult.SuccessResult();
    }

    protected override ExecuteResult<object?> ExecuteModuleTask(ImportSettings inputModel)
    {
        var executionResult = new ExecuteResult<object?>();

        var application = _configurationProvider.GetApplicationConfig();
        var designer = _metadataManager.GetDesigner(application.Id, "Services");
        if (designer == null)
        {
            executionResult.Errors.Add("Unable to find Services designer in application.");
            return executionResult;
        }

        var package = designer.Packages.FirstOrDefault(p => p.Id == inputModel.PackageId);
        if (package == null)
        {
            executionResult.Errors.Add($"Unable to find package with Id: {inputModel.PackageId}. Ensure the Service package is saved.");
            return executionResult;
        }

        var installedModules = _configurationProvider.GetInstalledModules().Select(m => m.ModuleId).ToList();

        if (!installedModules.Contains("Intent.Common.CSharp") && !installedModules.Contains("Intent.Common.Java"))
        {
            executionResult.Errors.Add("Install either 'Intent.Common.CSharp' or 'Intent.Common.Java' before running this import.");
            return executionResult;
        }

        var requiredModules = CalculateRequiredModules(inputModel, installedModules);
        var missingModules = requiredModules.Except(installedModules).ToList();
        if (missingModules.Any())
        {
            executionResult.Errors.Add($"Based on your selection, install the following modules:{Environment.NewLine}{string.Join(Environment.NewLine, missingModules)}");
            return executionResult;
        }

        var islnFile = Directory
            .GetFiles(_configurationProvider.GetSolutionConfig().SolutionRootLocation, "*.isln")
            .First();

        var serviceType = Enum.Parse<ServiceType>(inputModel.ServiceType, true);
        var persistence = Enum.TryParse<SettingPersistence>(inputModel.SettingPersistence, true, out var parsedPersistence)
            ? parsedPersistence
            : SettingPersistence.None;

        var config = new OpenApiImportConfig
        {
            IslnFile = islnFile,
            ApplicationName = application.Name,
            OpenApiSpecificationFile = inputModel.OpenApiSpecificationFile.Trim('"'),
            PackageId = package.Id,
            TargetFolderId = inputModel.TargetFolderId,
            AddPostFixes = inputModel.AddPostFixes,
            AllowRemoval = inputModel.AllowRemoval,
            IsAzureFunctions = application.Modules.Any(m => m.ModuleId == "Intent.AzureFunctions"),
            ServiceType = serviceType,
            SettingPersistence = persistence
        };

        Logging.Log.Info($"Starting OpenAPI import from {config.OpenApiSpecificationFile} into package {config.PackageId}");

        var factory = new OpenApiPersistableFactory();

        try
        {
            Helpers.ExecuteCore(
                intentSolutionPath: config.IslnFile,
                applicationName: config.ApplicationName,
                designerName: "Services",
                packageId: config.PackageId,
                targetFolderId: config.TargetFolderId,
                deleteExtra: config.AllowRemoval,
                debug: false,
                createAttributesWithUnknownTypes: true,
                stereotypeManagementMode: StereotypeManagementMode.Merge,
                additionalPreconditionChecks: null,
                getPersistables: packages => factory.GetPersistables(config, packages),
                persistAdditionalMetadata: factory.PersistAdditionalMetadata,
                packageTypeId: "df45eaf6-9202-4c25-8dd5-677e9ba1e906");

            executionResult.Warnings.AddRange(factory.Warnings);
        }
        catch (Exception exception)
        {
            Logging.Log.Failure(exception);
            executionResult.Errors.Add(exception.GetBaseException().Message);
        }

        return executionResult;
    }

    private static List<string> CalculateRequiredModules(ImportSettings settings, IList<string> installedModules)
    {
        var requiredModules = new List<string>();
        if (settings.ServiceType.Equals("CQRS", StringComparison.OrdinalIgnoreCase) && installedModules.Contains("Intent.Common.CSharp"))
        {
            requiredModules.Add("Intent.Modelers.Services.CQRS");
        }

        return requiredModules;
    }
}

