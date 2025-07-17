using System;
using System.Linq;
using Intent.Engine;
using Intent.Modules.SqlServerImporter.Tasks.Helpers;
using Intent.Modules.SqlServerImporter.Tasks.Models;
using Intent.Modules.SqlServerImporter.Tasks.Mappers;
using Intent.RelationalDbSchemaImporter.Contracts.Models;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.RelationalDbSchemaImporter.Runner;

namespace Intent.Modules.SqlServerImporter.Tasks;

public class RepositoryImport : ModuleTaskSingleInputBase<RepositoryImportModel>
{
    private readonly IMetadataManager _metadataManager;

    public RepositoryImport(IMetadataManager metadataManager)
    {
        _metadataManager = metadataManager;
    }

    public override string TaskTypeId => "Intent.Modules.SqlServerImporter.Tasks.StoredProcedureImport";
    public override string TaskTypeName => "SqlServer Stored Procedure Import";

    protected override ValidationResult ValidateInputModel(RepositoryImportModel inputModel)
    {
        var designer = _metadataManager.GetDesigner(inputModel.ApplicationId, "Domain");
        if (designer == null)
        {
            return ValidationResult.ErrorResult("Unable to find domain designer in application");
        }

        var package = designer.Packages.FirstOrDefault(p => p.Id == inputModel.PackageId);
        if (package == null)
        {
            return ValidationResult.ErrorResult($"Unable to find package with Id : {inputModel.PackageId}");
        }

        return ValidationResult.SuccessResult();
    }

    protected override ExecuteResult ExecuteModuleTask(RepositoryImportModel importModel)
    {
        PrepareInputModel(importModel);

        var executionResult = new ExecuteResult();
        
        // Step 1: Run CLI tool to get both package and schema data
        var result = ImporterTool.Run<ImportSchemaResult>("import-schema", importModel);

        if (result.Errors.Count > 0)
        {
            executionResult.Errors.AddRange(result.Errors);
            executionResult.Warnings.AddRange(result.Warnings);
            return executionResult;
        }

        // Step 2: If we have schema data, use our new mapping infrastructure
        if (result.Result?.SchemaData != null)
        {
            var mappingResult = ApplySchemaMapping(importModel, result.Result);
            if (!mappingResult.IsSuccessful)
            {
                executionResult.Errors.Add($"Schema mapping failed: {mappingResult.Message}");
                if (mappingResult.Exception != null)
                {
                    executionResult.Errors.Add($"Exception: {mappingResult.Exception.Message}");
                }
            }
        }

        // Step 3: Persist settings if successful
        if (result.Errors.Count == 0)
        {
            SettingsHelper.PersistSettings(importModel);
        }

        executionResult.Errors.AddRange(result.Errors);
        executionResult.Warnings.AddRange(result.Warnings);

        return executionResult;
    }

    private PackageUpdateResult ApplySchemaMapping(RepositoryImportModel importModel, ImportSchemaResult importResult)
    {
        try
        {
            // Get the package file path from the metadata manager
            if (!_metadataManager.TryGetApplicationPackage(importModel.ApplicationId, importModel.PackageId, out var packageMetadata, out var errorMessage))
            {
                return new PackageUpdateResult
                {
                    IsSuccessful = false,
                    Message = $"Could not retrieve package metadata: {errorMessage}"
                };
            }

            // Load the PackageModelPersistable directly from the file path
            var packageFilePath = packageMetadata.FileLocation;
            if (string.IsNullOrEmpty(packageFilePath))
            {
                return new PackageUpdateResult
                {
                    IsSuccessful = false,
                    Message = "Package file location is not available"
                };
            }

            var package = PackageModelPersistable.Load(packageFilePath);
            if (package == null)
            {
                return new PackageUpdateResult
                {
                    IsSuccessful = false,
                    Message = $"Could not load package from file: {packageFilePath}"
                };
            }

            // Create configuration for our mappers
            var config = CreateImportConfiguration(importModel);

            // Create the schema mapper
            var schemaMapper = new SchemaToIntentMapper(config);

            // Create deduplication context for this import operation
            var deduplicationContext = new DeduplicationContext();

            // Apply the mapping using the schema data from the CLI
            var mappingResult = schemaMapper.MapSchemaToPackage(importResult.SchemaData, package, deduplicationContext);

            // Save the package if mapping was successful
            if (mappingResult.IsSuccessful)
            {
                package.Save();
            }

            return mappingResult;
        }
        catch (Exception ex)
        {
            return new PackageUpdateResult
            {
                IsSuccessful = false,
                Message = $"Error during schema mapping: {ex.Message}",
                Exception = ex
            };
        }
    }

    private ImportConfiguration CreateImportConfiguration(RepositoryImportModel importModel)
    {
        return new ImportConfiguration
        {
            ApplicationId = importModel.ApplicationId,
            EntityNameConvention = Enum.Parse<Intent.RelationalDbSchemaImporter.Contracts.Enums.EntityNameConvention>(importModel.EntityNameConvention),
            TableStereotype = Enum.Parse<Intent.RelationalDbSchemaImporter.Contracts.Enums.TableStereotype>(importModel.TableStereotype),
            TypesToExport = importModel.TypesToExport.Select(t => Enum.Parse<Intent.RelationalDbSchemaImporter.Contracts.Enums.ExportType>(t)).ToHashSet(),
            StoredProcedureType = string.IsNullOrWhiteSpace(importModel.StoredProcedureType) 
                ? Intent.RelationalDbSchemaImporter.Contracts.Enums.StoredProcedureType.Default 
                : Enum.Parse<Intent.RelationalDbSchemaImporter.Contracts.Enums.StoredProcedureType>(importModel.StoredProcedureType),
            ConnectionString = importModel.ConnectionString,
            PackageFileName = importModel.PackageFileName,
            RepositoryElementId = importModel.RepositoryElementId,
            StoredProcNames = importModel.StoredProcNames
        };
    }

    private void PrepareInputModel(RepositoryImportModel importModel)
    {
        // After validation, we can safely assume the following lookups:
        var designer = _metadataManager.GetDesigner(importModel.ApplicationId, "Domain");
        var package = designer.Packages.First(p => p.Id == importModel.PackageId);

        importModel.EntityNameConvention = "SingularEntity";
        importModel.TableStereotype = "WhenDifferent";
        importModel.TypesToExport = ["StoredProcedure"];
        importModel.PackageFileName = package.FileLocation;

        if (importModel.SettingPersistence == RepositorySettingPersistence.InheritDb)
        {
            SettingsHelper.HydrateDbSettings(importModel);
        }

        if (string.IsNullOrWhiteSpace(importModel.StoredProcedureType))
        {
            importModel.StoredProcedureType = "Default";
        }
    }
}