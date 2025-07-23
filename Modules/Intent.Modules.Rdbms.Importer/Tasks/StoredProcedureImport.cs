using System;
using System.Linq;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Modules.Rdbms.Importer.Tasks.Helpers;
using Intent.Modules.Rdbms.Importer.Tasks.Mappers;
using Intent.Modules.Rdbms.Importer.Tasks.Models;
using Intent.RelationalDbSchemaImporter.Contracts.Commands;
using Intent.RelationalDbSchemaImporter.Contracts.Enums;
using Intent.RelationalDbSchemaImporter.Runner;

namespace Intent.Modules.Rdbms.Importer.Tasks;

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
        var executionResult = new ExecuteResult();
        try
        {
            PrepareInputModel(importModel);
            
            var result = ImporterTool.Run<ImportSchemaResult>("import-schema", importModel);

            executionResult.Errors.AddRange(result.Errors);
            executionResult.Warnings.AddRange(result.Warnings);

            if (executionResult.Errors.Count > 0 || result.Result?.SchemaData == null)
            {
                return executionResult;
            }

            var mappingResult = ApplySchemaMapping(importModel, result.Result);
            if (mappingResult.IsSuccessful)
            {
                return executionResult;
            }

            executionResult.Errors.Add($"Schema mapping failed: {mappingResult.Message}");
            if (mappingResult.Exception != null)
            {
                executionResult.Errors.Add($"Exception: {mappingResult.Exception.Message}");
            }
        }
        finally
        {
            SettingsHelper.PersistSettings(importModel);
        }

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
            var schemaMapper = new DbSchemaIntentMetadataMerger(config);

            // Create deduplication context for this import operation
            var deduplicationContext = new DeduplicationContext();

            // Apply the mapping using the schema data from the CLI
            var mappingResult = schemaMapper.MergeSchemaAndPackage(importResult.SchemaData, package, deduplicationContext);

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
    
    private static ImportConfiguration CreateImportConfiguration(RepositoryImportModel importModel)
    {
        return new ImportConfiguration
        {
            ApplicationId = importModel.ApplicationId,
            ConnectionString = importModel.ConnectionString,
            PackageFileName = importModel.PackageFileName,
            RepositoryElementId = importModel.RepositoryElementId,
            StoredProcNames = importModel.StoredProcNames,
            EntityNameConvention = Enum.Parse<EntityNameConvention>(importModel.EntityNameConvention),
            TableStereotype = Enum.Parse<TableStereotype>(importModel.TableStereotype),
            TypesToExport = importModel.TypesToExport.Select(Enum.Parse<ExportType>).ToHashSet(),
            StoredProcedureType = string.IsNullOrWhiteSpace(importModel.StoredProcedureType)
                ? StoredProcedureType.Default
                : Enum.Parse<StoredProcedureType>(importModel.StoredProcedureType),
            DatabaseType = importModel.DatabaseType ?? throw new Exception("Database type is required for repository import.")
        };
    }
}