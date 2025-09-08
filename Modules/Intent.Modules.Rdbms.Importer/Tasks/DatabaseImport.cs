using System;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.IArchitect.CrossPlatform.IO;
using Intent.Modules.Rdbms.Importer.Tasks.Helpers;
using Intent.Modules.Rdbms.Importer.Tasks.Mappers;
using Intent.Modules.Rdbms.Importer.Tasks.Models;
using Intent.Plugins;
using Intent.RelationalDbSchemaImporter.Contracts.Commands;
using Intent.RelationalDbSchemaImporter.Contracts.Enums;
using Intent.RelationalDbSchemaImporter.Runner;
using Intent.Utils;

namespace Intent.Modules.Rdbms.Importer.Tasks;

public class DatabaseImport : IModuleTask
{
    private readonly IMetadataManager _metadataManager;
    private readonly IApplicationConfigurationProvider _configurationProvider;

    public DatabaseImport(IMetadataManager metadataManager, IApplicationConfigurationProvider configurationProvider)
    {
        _metadataManager = metadataManager;
        _configurationProvider = configurationProvider;
    }

    public string TaskTypeId => "Intent.Modules.Rdbms.Importer.Tasks.DatabaseImport";
    public string TaskTypeName => "SqlServer Database Import";
    public int Order => 0;

    public string? Execute(params string[] args)
    {
        var importModel = JsonSerializer.Deserialize<DatabaseImportModel>(args[0], SerializationHelper.SerializerOptions);
        if (importModel == null)
        {
            throw new Exception(
                $"""
                 Deserialization of the following returned null:
                 
                 {args[0]}
                 """);
        }
        if (!_metadataManager.TryGetApplicationPackage(importModel.ApplicationId, importModel.PackageId, out _, out var errorMessage))
        {
            throw new Exception(errorMessage);
        }

        ImporterTool.SetToolDirectory(Path.GetFullPath(Path.Combine(
            Path.GetDirectoryName(typeof(ImporterTool).Assembly.Location)!,
            "../content/tool")));

        try
        {
            var adaptedImportModel = PrepareInputModel(importModel);
            var result = ImporterTool.Run<ImportSchemaResult>("import-schema", adaptedImportModel);

            foreach (var message in result.Errors)
            {
                Logging.Log.Failure(message);
            }

            foreach (var message in result.Warnings)
            {
                Logging.Log.Warning(message);
            }

            if (result.Errors.Count > 0 || result.Result?.SchemaData is null)
            {
                throw new Exception("One or more errors occurred, review previous log entries");
            }

            var mappingResult = ApplySchemaMapping(importModel, result.Result);

            foreach (var message in mappingResult.Warnings)
            {
                Logging.Log.Warning(message);
            }

            if (mappingResult.Exception != null)
            {
                throw new Exception($"Schema mapping failed: {mappingResult.Message}", mappingResult.Exception);
            }

            if (!mappingResult.IsSuccessful)
            {
                throw new Exception("When applying schema mapping a non-success result was received.");
            }
        }
        finally
        {
            SettingsHelper.PersistSettings(importModel);
        }

        return null;
    }

    private DatabaseImportModel PrepareInputModel(DatabaseImportModel inputModel)
    {
        if (string.IsNullOrWhiteSpace(inputModel.StoredProcedureType))
        {
            inputModel.StoredProcedureType = "Default";
        }

        if (string.IsNullOrWhiteSpace(inputModel.AttributeNameConvention))
        {
            inputModel.AttributeNameConvention = "LanguageCompliant";
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

    private MergeResult ApplySchemaMapping(DatabaseImportModel importModel, ImportSchemaResult importResult)
    {
        try
        {
            // Get the package file path from the metadata manager
            if (!_metadataManager.TryGetApplicationPackage(importModel.ApplicationId, importModel.PackageId, out var packageMetadata, out var errorMessage))
            {
                return new MergeResult
                {
                    IsSuccessful = false,
                    Message = $"Could not retrieve package metadata: {errorMessage}"
                };
            }

            // Load the PackageModelPersistable directly from the file path
            var packageFilePath = packageMetadata.FileLocation;
            if (string.IsNullOrEmpty(packageFilePath))
            {
                return new MergeResult
                {
                    IsSuccessful = false,
                    Message = "Package file location is not available"
                };
            }

            var package = PackageModelPersistable.Load(packageFilePath);
            if (package == null)
            {
                return new MergeResult
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
            return new MergeResult
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
            AttributeNameConvention = Enum.Parse<AttributeNameConvention>(importModel.AttributeNameConvention),
            TableStereotype = Enum.Parse<TableStereotype>(importModel.TableStereotype),
            TypesToExport = importModel.TypesToExport.Select(Enum.Parse<ExportType>).ToHashSet(),
            StoredProcedureType = string.IsNullOrWhiteSpace(importModel.StoredProcedureType)
                ? StoredProcedureType.Default
                : Enum.Parse<StoredProcedureType>(importModel.StoredProcedureType),
            DatabaseType = importModel.DatabaseType
        };
    }
}
