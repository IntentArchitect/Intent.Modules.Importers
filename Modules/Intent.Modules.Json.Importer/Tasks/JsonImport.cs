using System;
using System.IO;
using System.Linq;
using Intent.Engine;
using Intent.MetadataSynchronizer.Configuration;
using Intent.MetadataSynchronizer.Json.CLI;
using Intent.Modules.Json.Importer.Tasks.Helpers;
using Intent.Modules.Json.Importer.Tasks.Models;
using Intent.Utils;

namespace Intent.Modules.Json.Importer.Tasks;

public class JsonImport : ModuleTaskBase<JsonImportInputModel>
{
    private readonly IMetadataManager _metadataManager;
    private readonly IApplicationConfigurationProvider _configurationProvider;

    public JsonImport(IMetadataManager metadataManager, IApplicationConfigurationProvider configurationProvider)
    {
        _metadataManager = metadataManager;
        _configurationProvider = configurationProvider;
    }

    public override string TaskTypeId => "Intent.Modules.Json.Importer.Tasks.JsonImport";
    public override string TaskTypeName => "JSON Import";

    protected override ValidationResult ValidateInputModel(JsonImportInputModel inputModel)
    {
        if (string.IsNullOrWhiteSpace(inputModel.SourceFolder))
            return ValidationResult.ErrorResult("Source folder is required.");

        if (!Directory.Exists(inputModel.SourceFolder))
            return ValidationResult.ErrorResult($"Source folder does not exist: {inputModel.SourceFolder}");

        if (string.IsNullOrWhiteSpace(inputModel.PackageId))
            return ValidationResult.ErrorResult("Package ID is required.");

        if (inputModel.Profile == default)
            return ValidationResult.ErrorResult("Profile is required.");

        if (!Enum.IsDefined(typeof(ImportProfile), inputModel.Profile))
            return ValidationResult.ErrorResult($"Profile '{inputModel.Profile}' is not valid.");

        // Validate that the package exists - use application config instead of input application ID
        var application = _configurationProvider.GetApplicationConfig();
        if (!_metadataManager.TryGetApplicationPackage(application.Id, ProfileFactory.GetSettings(inputModel.Profile).DesignerName, inputModel.PackageId, out _, out var errorMessage))
        {
            return ValidationResult.ErrorResult($"Package validation failed: {errorMessage}");
        }

        return ValidationResult.SuccessResult();
    }

    protected override ExecuteResult ExecuteModuleTask(JsonImportInputModel inputModel)
    {
        var executionResult = new ExecuteResult();

        // Get .isln file path
        var islnFile = Directory.GetFiles(_configurationProvider.GetSolutionConfig().SolutionRootLocation, "*.isln").First();

        // Get application config
        var application = _configurationProvider.GetApplicationConfig();

        // Build JsonConfig from UI input
        var config = new JsonConfig
        {
            SourceJsonFolder = inputModel.SourceFolder,
            IslnFile = islnFile,
            ApplicationName = application.Name,
            PackageId = inputModel.PackageId,
            TargetFolderId = inputModel.TargetFolderId,
            CasingConvention = inputModel.CasingConvention,
            Profile = inputModel.Profile
        };

        // Resolve settings from profile (same as CLI does)
        var settings = ProfileFactory.GetSettings(config.Profile);

        Logging.Log.Info($"Starting JSON import from {config.SourceJsonFolder} into package {config.PackageId}");

        // Invoke the core synchronizer logic directly (no process spawn)
        Intent.MetadataSynchronizer.Helpers.Execute(
            intentSolutionPath: config.IslnFile,
            applicationName: config.ApplicationName,
            designerName: settings.DesignerName,
            packageId: config.PackageId,
            targetFolderId: config.TargetFolderId,
            deleteExtra: false, // Default; can add UI toggle later
            debug: false,
            createAttributesWithUnknownTypes: true,
            stereotypeManagementMode: StereotypeManagementMode.Merge,
            additionalPreconditionChecks: null,
            getPersistables: packages => inputModel.SelectedFiles.Count > 0
                ? JsonPersistableFactory.GetPersistables(config, packages, inputModel.SelectedFiles)
                : JsonPersistableFactory.GetPersistables(config, packages));

        return executionResult;
    }
}