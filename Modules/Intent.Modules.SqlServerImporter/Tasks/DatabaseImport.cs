using System;
using Intent.Engine;
using Intent.Modules.SqlServerImporter.Tasks.Helpers;
using Intent.Modules.SqlServerImporter.Tasks.Models;
using Intent.Modules.SqlServerImporter.Tasks.Mappers;
using Intent.RelationalDbSchemaImporter.Contracts.Models;
using Intent.IArchitect.Agent.Persistence.Model.Common;

namespace Intent.Modules.SqlServerImporter.Tasks;

public class DatabaseImport : ModuleTaskSingleInputBase<DatabaseImportModel>
{
    private readonly IMetadataManager _metadataManager;

    public DatabaseImport(IMetadataManager metadataManager)
    {
        _metadataManager = metadataManager;
    }

    public override string TaskTypeId => "Intent.Modules.SqlServerImporter.Tasks.DatabaseImport";
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
            executionResult.Warnings.Add($"Schema data received: {result.Result.SchemaData.Tables.Count} tables, {result.Result.SchemaData.Views.Count} views, {result.Result.SchemaData.StoredProcedures.Count} stored procedures");
            
            var mappingResult = ApplySchemaMapping(importModel, result.Result);
            if (!mappingResult.IsSuccessful)
            {
                executionResult.Errors.Add($"Schema mapping failed: {mappingResult.Message}");
                if (mappingResult.Exception != null)
                {
                    executionResult.Errors.Add($"Exception: {mappingResult.Exception.Message}");
                }
            }
            else
            {
                executionResult.Warnings.Add($"Schema mapping completed: {mappingResult.Message}");
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
            var config = SchemaToIntentMapper.CreateImportConfiguration(importModel);

            // Create the schema mapper
            var schemaMapper = new SchemaToIntentMapper(config);

            // Apply the mapping using the schema data from the CLI
            var mappingResult = schemaMapper.MapSchemaToPackage(importResult.SchemaData, package);

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

    private void PrepareInputModel(DatabaseImportModel inputModel)
    {
        if (!_metadataManager.TryGetApplicationPackage(inputModel.ApplicationId, inputModel.PackageId, out var package, out _))
        {
            return;
        }

        // Making required changes for the underlying CLI tool
        
        inputModel.PackageFileName = package.FileLocation;

        if (string.IsNullOrWhiteSpace(inputModel.StoredProcedureType))
        {
            inputModel.StoredProcedureType = "Default";
        }
    }
}
