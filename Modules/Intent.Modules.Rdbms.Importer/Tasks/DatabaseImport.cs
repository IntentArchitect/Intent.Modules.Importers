using System;
using System.Linq;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.IArchitect.CrossPlatform.IO;
using Intent.Modules.Rdbms.Importer.Tasks.Helpers;
using Intent.Modules.Rdbms.Importer.Tasks.Mappers;
using Intent.Modules.Rdbms.Importer.Tasks.Models;
using Intent.RelationalDbSchemaImporter.Contracts.Commands;
using Intent.RelationalDbSchemaImporter.Contracts.Enums;
using Intent.RelationalDbSchemaImporter.Runner;

namespace Intent.Modules.Rdbms.Importer.Tasks;

public class DatabaseImport : ModuleTaskSingleInputBase<DatabaseImportModel>
{
    private readonly IMetadataManager _metadataManager;
    private readonly IApplicationConfigurationProvider _configurationProvider;

    public DatabaseImport(IMetadataManager metadataManager, IApplicationConfigurationProvider configurationProvider)
    {
        _metadataManager = metadataManager;
        _configurationProvider = configurationProvider;
    }

    public override string TaskTypeId => "Intent.Modules.Rdbms.Importer.Tasks.DatabaseImport";
    public override string TaskTypeName => "SqlServer Database Import";

    protected override ValidationResult ValidateInputModel(DatabaseImportModel inputModel)
    {
        if (!_metadataManager.TryGetApplicationPackage(inputModel.ApplicationId, inputModel.PackageId, out _, out var errorMessage))
        {
            return ValidationResult.ErrorResult(errorMessage);
        }

        return ValidationResult.SuccessResult();
    }

    protected override ExecuteResult ExecuteModuleTask(DatabaseImportModel importModel)
    {
        var executionResult = new ExecuteResult();
        try
        {
            var adaptedImportModel = PrepareInputModel(importModel);
            var result = ImporterTool.Run<ImportSchemaResult>("import-schema", adaptedImportModel);

            executionResult.Errors.AddRange(result.Errors);
            executionResult.Warnings.AddRange(result.Warnings);

            if (executionResult.Errors.Count > 0 || result.Result?.SchemaData is null)
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
    
    private DatabaseImportModel PrepareInputModel(DatabaseImportModel inputModel)
    {
        if (string.IsNullOrWhiteSpace(inputModel.StoredProcedureType))
        {
            inputModel.StoredProcedureType = "Default";
        }
        
        if (!_metadataManager.TryGetApplicationPackage(inputModel.ApplicationId, inputModel.PackageId, out var package, out _))
        {
            throw new Exception($"Package {inputModel.PackageId} for Application {inputModel.ApplicationId} doesn't exist");
        }
        
        inputModel.PackageFileName = package.FileLocation;

        // Making required changes for the underlying CLI tool
        var adaptedModel = new DatabaseImportModel(inputModel);
        
        if (!string.IsNullOrWhiteSpace(adaptedModel.ImportFilterFilePath) &&
            !Path.IsPathRooted(adaptedModel.ImportFilterFilePath))
        {
            adaptedModel.ImportFilterFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(adaptedModel.PackageFileName)!, adaptedModel.ImportFilterFilePath));
        }

        return adaptedModel;
    }

    private PackageUpdateResult ApplySchemaMapping(DatabaseImportModel importModel, ImportSchemaResult importResult)
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
            
            ModuleHelper.ApplyPackageStereotypes(package, _configurationProvider);
            ModuleHelper.ApplyRelevantReferences(package, _configurationProvider);
            
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
    
    private static ImportConfiguration CreateImportConfiguration(DatabaseImportModel importModel)
    {
        return new ImportConfiguration
        {
            ApplicationId = importModel.ApplicationId,
            ConnectionString = importModel.ConnectionString,
            PackageFileName = importModel.PackageFileName,
            ImportFilterFilePath = importModel.ImportFilterFilePath,
            EntityNameConvention = Enum.Parse<EntityNameConvention>(importModel.EntityNameConvention),
            TableStereotype = Enum.Parse<TableStereotype>(importModel.TableStereotype),
            TypesToExport = importModel.TypesToExport.Select(Enum.Parse<ExportType>).ToHashSet(),
            StoredProcedureType = string.IsNullOrWhiteSpace(importModel.StoredProcedureType)
                ? StoredProcedureType.Default
                : Enum.Parse<StoredProcedureType>(importModel.StoredProcedureType),
            DatabaseType = importModel.DatabaseType
        };
    }
}
