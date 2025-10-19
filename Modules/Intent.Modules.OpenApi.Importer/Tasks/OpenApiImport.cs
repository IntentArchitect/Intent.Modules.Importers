using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.MetadataSynchronizer.Configuration;
using Intent.Modules.Json.Importer.Tasks.Helpers;
using Intent.Modules.Json.Importer.Tasks.Models;
using Intent.Modules.OpenApi.Importer.Importer;
using Intent.Utils;

namespace Intent.Modules.OpenApi.Importer.Tasks
{
    public class OpenApiImport : ModuleTaskBase<OpenApiImportInputModel, object>
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

        protected override ValidationResult ValidateInputModel(OpenApiImportInputModel inputModel)
        {
            if (string.IsNullOrWhiteSpace(inputModel.OpenApiSpecificationFile))
                return ValidationResult.ErrorResult("OpenAPI specification file is required.");

            if (!File.Exists(inputModel.OpenApiSpecificationFile) && !inputModel.OpenApiSpecificationFile.StartsWith("http"))
                return ValidationResult.ErrorResult($"OpenAPI specification file does not exist: {inputModel.OpenApiSpecificationFile}");

            if (string.IsNullOrWhiteSpace(inputModel.PackageId))
                return ValidationResult.ErrorResult("Package ID is required.");

            // Validate that the package exists
            var application = _configurationProvider.GetApplicationConfig();
            if (!_metadataManager.TryGetApplicationPackage(application.Id, "Services", inputModel.PackageId, out _, out var errorMessage))
            {
                return ValidationResult.ErrorResult($"Package validation failed: {errorMessage}");
            }

            // Validate required modules
            var installedModules = _configurationProvider.GetInstalledModules().Select(m => m.ModuleId).ToList();

            if (!installedModules.Contains("Intent.Common.CSharp") && !installedModules.Contains("Intent.Common.Java"))
            {
                return ValidationResult.ErrorResult("You need to Install the `Intent.Common.CSharp` OR `Intent.Common.Java` module.");
            }

            var requiredModules = CalculateRequiredModules(inputModel, installedModules);

            if (requiredModules.Any() && requiredModules.Except(installedModules).Any())
            {
                var missingModules = requiredModules.Except(installedModules);
                return ValidationResult.ErrorResult($"Based on your selection, you need to Install the following modules.\n{string.Join("\n", missingModules)}");
            }

            return ValidationResult.SuccessResult();
        }

        protected override ExecuteResult<object> ExecuteModuleTask(OpenApiImportInputModel inputModel)
        {
            var executionResult = new ExecuteResult<object>();

            // Get .isln file path
            var islnFile = Directory.GetFiles(_configurationProvider.GetSolutionConfig().SolutionRootLocation, "*.isln").First();

            // Get application config
            var application = _configurationProvider.GetApplicationConfig();

            // Build ImportConfig from UI input
            var config = new ImportConfig
            {
                OpenApiSpecificationFile = inputModel.OpenApiSpecificationFile.Trim('"'),
                IslnFile = islnFile,
                ApplicationName = application.Name,
                PackageId = inputModel.PackageId,
                TargetFolderId = inputModel.TargetFolderId,
                ServiceType = inputModel.ServiceType,
                AddPostFixes = inputModel.AddPostFixes,
                IsAzureFunctions = inputModel.IsAzureFunctions || application.Modules.Any(m => m.ModuleId == "Intent.AzureFunctions"),
                AllowRemoval = inputModel.AllowRemoval,
                SettingPersistence = inputModel.SettingPersistence,
                ReverseEngineerImplementation = inputModel.ReverseEngineerImplementation,
                DomainPackageId = inputModel.DomainPackageId
            };

            Logging.Log.Info($"Starting OpenAPI import from {config.OpenApiSpecificationFile} into package {config.PackageId}");

            // Create factory
            var factory = new OpenApiPersistableFactory();

            // Invoke the core synchronizer logic directly (no process spawn)
            Intent.MetadataSynchronizer.Helpers.Execute(
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
                persistAdditionalMetadata: package => factory.PersistAdditionalMetadata(package),
                packageTypeId: "df45eaf6-9202-4c25-8dd5-677e9ba1e906");

            // Handle domain reverse engineering if enabled
            if (config.ReverseEngineerImplementation && !string.IsNullOrWhiteSpace(config.DomainPackageId))
            {
                Logging.Log.Info($"Reverse engineering domain implementation into package {config.DomainPackageId}");
                
                Intent.MetadataSynchronizer.Helpers.Execute(
                    intentSolutionPath: config.IslnFile,
                    applicationName: config.ApplicationName,
                    designerName: "Domain",
                    packageId: config.DomainPackageId,
                    targetFolderId: null,
                    deleteExtra: false,
                    debug: false,
                    createAttributesWithUnknownTypes: true,
                    stereotypeManagementMode: StereotypeManagementMode.Merge,
                    additionalPreconditionChecks: null,
                    getPersistables: packages => factory.GetDomainPersistables(),
                    persistAdditionalMetadata: null,
                    packageTypeId: "1a824508-4623-45d9-accc-f572091ade5a");
            }

            return executionResult;
        }

        private List<string> CalculateRequiredModules(OpenApiImportInputModel inputModel, IList<string> installedModules)
        {
            var requiredModules = new List<string>();
            if (inputModel.ServiceType == ServiceType.CQRS)
            {
                if (installedModules.Contains("Intent.Common.CSharp"))
                {
                    requiredModules.Add("Intent.Modelers.Services.CQRS");
                }
            }
            return requiredModules;
        }
    }
}

