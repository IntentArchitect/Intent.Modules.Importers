using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Modules.Rdbms.Importer.Tasks.Helpers;
using Intent.Modules.Rdbms.Importer.Tasks.Mappers;
using Intent.Modules.Rdbms.Importer.Tasks.Models;
using Intent.Plugins;
using Intent.RelationalDbSchemaImporter.Contracts.Commands;
using Intent.RelationalDbSchemaImporter.Contracts.Enums;
using Intent.RelationalDbSchemaImporter.Runner;
using Intent.Utils;

namespace Intent.Modules.Rdbms.Importer.Tasks;

public class RepositoryImport : IModuleTask
{
    private readonly IMetadataManager _metadataManager;
    private readonly IApplicationConfigurationProvider _configurationProvider;

    public RepositoryImport(IMetadataManager metadataManager, IApplicationConfigurationProvider configurationProvider)
    {
        _metadataManager = metadataManager;
        _configurationProvider = configurationProvider;
    }

    public string TaskTypeId => "Intent.Modules.Rdbms.Importer.Tasks.StoredProcedureImport";
    public string TaskTypeName => "SqlServer Stored Procedure Import";
    public int Order => 0;

    public string? Execute(params string[] args)
    {
        var importModel = JsonSerializer.Deserialize<RepositoryImportModel>(args[0], SerializationHelper.SerializerOptions);
        if (importModel == null)
        {
            throw new Exception(
                $"""
                 Deserialization of the following returned null:

                 {args[0]}
                 """);
        }

        var designer = _metadataManager.GetDesigner(importModel.ApplicationId, "Domain");
        if (designer == null)
        {
            throw new Exception("Unable to find domain designer in application");
        }

        var package = designer.Packages.FirstOrDefault(p => p.Id == importModel.PackageId);
        if (package == null)
        {
            throw new Exception($"Unable to find package with Id : {importModel.PackageId}");
        }

        ImporterTool.SetToolDirectory(Path.GetFullPath(Path.Combine(
            Path.GetDirectoryName(typeof(ImporterTool).Assembly.Location)!,
            "../content/tool")));

        try
        {
            PrepareInputModel(importModel);

            var result = ImporterTool.Run<ImportSchemaResult>("import-schema", importModel);

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

    private MergeResult ApplySchemaMapping(RepositoryImportModel importModel, ImportSchemaResult importResult)
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