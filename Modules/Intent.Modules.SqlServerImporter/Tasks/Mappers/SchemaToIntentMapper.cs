using System;
using System.Collections.Generic;
using System.Linq;
using Intent.RelationalDbSchemaImporter.Contracts.Enums;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Modules.SqlServerImporter.Tasks.Models;
using Intent.RelationalDbSchemaImporter.Contracts.DbSchema;

namespace Intent.Modules.SqlServerImporter.Tasks.Mappers;

internal class SchemaToIntentMapper
{
    private readonly IntentModelMapper _intentModelMapper;
    private readonly ImportConfiguration _config;

    // Track schema folders to avoid duplicates
    private readonly Dictionary<string, ElementPersistable> _schemaFolders = new();

    public SchemaToIntentMapper(ImportConfiguration config)
    {
        _config = config;
        _intentModelMapper = new IntentModelMapper();
    }

    public PackageUpdateResult MapSchemaToPackage(DatabaseSchema databaseSchema, PackageModelPersistable package, DeduplicationContext? deduplicationContext = null)
    {
        var result = new PackageUpdateResult();

        try
        {
            // Clear existing elements to rebuild from schema
            var existingElements = package.Classes.ToList();

            // Map tables to classes
            if (_config.ExportTables())
            {
                foreach (var table in databaseSchema.Tables)
                {
                    // Check if class already exists first (before creating folder structure)
                    // Use ExternalReference for robust lookup first, then fallback to name-based
                    var tableExternalRef = IntentModelMapper.GetTableExternalReference(table.Name, table.Schema);
                    var existingClass = existingElements.FirstOrDefault(c =>
                                            c.ExternalReference == tableExternalRef) ??
                                        existingElements.FirstOrDefault(c =>
                                            IsTableMappedToClass(c, table));

                    if (existingClass != null)
                    {
                        // Update existing class without moving it (keep existing ParentFolderId)
                        // Don't use deduplication context for updates to preserve existing names
                        var updatedClassElement = _intentModelMapper.MapTableToClass(table, _config, existingClass.ParentFolderId);
                        UpdateExistingClass(existingClass, updatedClassElement);
                        result.UpdatedElements.Add(existingClass);

                        // Process indexes for existing class
                        ProcessTableIndexes(table, existingClass, package);
                    }
                    else
                    {
                        // Only create schema folder for new elements
                        var schemaFolder = GetOrCreateSchemaFolder(table.Schema, package);
                        // Use deduplication context for new elements only
                        var classElement = _intentModelMapper.MapTableToClass(table, _config, schemaFolder.Id, deduplicationContext);

                        package.Classes.Add(classElement);
                        result.AddedElements.Add(classElement);

                        // Process indexes for new class
                        ProcessTableIndexes(table, classElement, package);
                    }
                }
            }

            // Map views to classes
            if (_config.ExportViews())
            {
                foreach (var view in databaseSchema.Views)
                {
                    // Check if class already exists first (before creating folder structure)
                    // Use ExternalReference for robust lookup first, then fallback to name-based
                    var viewExternalRef = IntentModelMapper.GetViewExternalReference(view.Name, view.Schema);
                    var existingClass = existingElements.FirstOrDefault(c =>
                                            c.ExternalReference == viewExternalRef) ??
                                        existingElements.FirstOrDefault(c =>
                                            IsViewMappedToClass(c, view));

                    if (existingClass != null)
                    {
                        // Update existing class without moving it (keep existing ParentFolderId)
                        // Don't use deduplication context for updates to preserve existing names
                        var updatedClassElement = _intentModelMapper.MapViewToClass(view, _config, existingClass.ParentFolderId);
                        UpdateExistingClass(existingClass, updatedClassElement);
                        result.UpdatedElements.Add(existingClass);
                    }
                    else
                    {
                        // Only create schema folder for new elements
                        var schemaFolder = GetOrCreateSchemaFolder(view.Schema, package);
                        // Use deduplication context for new elements only
                        var classElement = _intentModelMapper.MapViewToClass(view, _config, schemaFolder.Id, deduplicationContext);

                        package.Classes.Add(classElement);
                        result.AddedElements.Add(classElement);
                    }
                }
            }

            // Map stored procedures
            if (_config.ExportStoredProcedures())
            {
                foreach (var storedProc in databaseSchema.StoredProcedures)
                {
                    ElementPersistable procElement;

                    // Create stored procedure element based on configuration
                    if (_config.StoredProcedureType == StoredProcedureType.StoredProcedureElement)
                    {
                        // Create repository first if it doesn't exist
                        var repositoryElement = GetOrCreateRepository(_config.RepositoryElementId, storedProc.Schema, package);

                        // Create as stored procedure element
                        procElement = _intentModelMapper.MapStoredProcedureToElement(storedProc, repositoryElement.Id, _config, deduplicationContext);
                        repositoryElement.ChildElements.Add(procElement);

                        // Apply stored procedure stereotypes
                        RdbmsSchemaAnnotator.ApplyStoredProcedureSettings(storedProc, procElement);

                        result.AddedElements.Add(procElement);
                    }
                    else
                    {
                        // Create repository first if it doesn't exist
                        var repositoryElement = GetOrCreateRepository(_config.RepositoryElementId, storedProc.Schema, package);

                        // Create as operation within repository
                        procElement = _intentModelMapper.MapStoredProcedureToOperation(storedProc, repositoryElement.Id, _config, deduplicationContext);
                        repositoryElement.ChildElements.Add(procElement);

                        // Apply stored procedure stereotypes
                        RdbmsSchemaAnnotator.ApplyStoredProcedureSettings(storedProc, procElement);

                        result.AddedElements.Add(procElement);
                    }

                    // Create data contract if stored procedure has result set
                    if (storedProc.ResultSetColumns.Count > 0)
                    {
                        ProcessStoredProcedureDataContract(storedProc, procElement, package, existingElements, result, deduplicationContext);
                    }
                }
            }

            // Process foreign keys AFTER all classes are created
            ProcessForeignKeys(databaseSchema, package, result);

            result.IsSuccessful = true;
            result.Message = $"Successfully mapped {result.AddedElements.Count} new elements and updated {result.UpdatedElements.Count} existing elements.";
        }
        catch (Exception ex)
        {
            result.IsSuccessful = false;
            result.Message = $"Error mapping schema to package: {ex.Message}";
            result.Exception = ex;
        }

        return result;
    }

    public static ImportConfiguration CreateImportConfiguration(DatabaseImportModel importModel)
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

    public static ImportConfiguration CreateImportConfiguration(RepositoryImportModel importModel)
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
            DatabaseType = importModel.DatabaseType
        };
    }

    /// <summary>
    /// Processes table indexes and creates them in the package
    /// </summary>
    private void ProcessTableIndexes(TableSchema table, ElementPersistable classElement, PackageModelPersistable package)
    {
        foreach (var index in table.Indexes)
        {
            var indexElement = _intentModelMapper.CreateIndex(index, classElement.Id, package);
            package.Classes.Add(indexElement);

            // Create index columns
            foreach (var indexColumn in index.Columns)
            {
                // Find the corresponding attribute in the class
                var attribute = classElement.ChildElements.FirstOrDefault(attr =>
                    attr.Name.Equals(indexColumn.Name, StringComparison.OrdinalIgnoreCase));

                var indexColumnElement = _intentModelMapper.CreateIndexColumn(indexColumn, indexElement.Id, attribute?.Id, package);
                indexElement.ChildElements.Add(indexColumnElement);
            }
        }
    }

    private static bool IsTableMappedToClass(ElementPersistable classElement, TableSchema table)
    {
        // Check if class has table stereotype with matching table name
        return classElement.Stereotypes?.Any(s =>
            s.DefinitionId == Constants.Stereotypes.Rdbms.Table.DefinitionId &&
            s.Properties?.Any(p => p.Value == table.Name) == true) == true;
    }

    private static bool IsViewMappedToClass(ElementPersistable classElement, ViewSchema view)
    {
        // Check if class has view stereotype with matching view name
        return classElement.Stereotypes?.Any(s =>
            s.DefinitionId == Constants.Stereotypes.Rdbms.View.DefinitionId &&
            s.Properties?.Any(p => p.Value == view.Name) == true) == true;
    }

    private static void UpdateExistingClass(ElementPersistable existingClass, ElementPersistable newClass)
    {
        // Update stereotypes
        existingClass.Stereotypes = newClass.Stereotypes;

        // Update child elements (attributes)
        existingClass.ChildElements = newClass.ChildElements;

        // Update type reference if needed
        if (newClass.TypeReference != null)
        {
            existingClass.TypeReference = newClass.TypeReference;
        }
    }

    /// <summary>
    /// Gets or creates a schema folder for organizing elements by database schema
    /// </summary>
    private ElementPersistable GetOrCreateSchemaFolder(string schemaName, PackageModelPersistable package)
    {
        if (_schemaFolders.TryGetValue(schemaName, out var existingFolder))
            return existingFolder;

        // Check if folder already exists in package
        var folder = package.Classes.FirstOrDefault(c =>
            c.Name == GetNormalizedSchemaName(schemaName) &&
            c.SpecializationType == "Folder");

        if (folder == null)
        {
            folder = _intentModelMapper.CreateSchemaFolder(schemaName, package.Id);
            package.Classes.Add(folder);
        }

        _schemaFolders[schemaName] = folder;
        return folder;
    }

    private static string GetNormalizedSchemaName(string schemaName)
    {
        return schemaName.Substring(0, 1).ToUpper() + schemaName.Substring(1);
    }

    /// <summary>
    /// Gets or creates a repository element for stored procedure operations
    /// </summary>
    private ElementPersistable GetOrCreateRepository(string? repositoryElementId, string schemaName, PackageModelPersistable package)
    {
        var repositoryName = $"{GetNormalizedSchemaName(schemaName)}Repository";

        // Check if repository already exists
        var repository = !string.IsNullOrWhiteSpace(repositoryElementId)
            ? package.Classes.FirstOrDefault(x => x.Id == repositoryElementId) 
              ?? throw new Exception($"Selected Repository could not be found. Did you save your designer before running the importer?")
            : package.Classes.FirstOrDefault(c => c.Name == repositoryName && c.SpecializationType == "Repository");

        if (repository == null)
        {
            // Get or create schema folder for repository organization
            var schemaFolder = GetOrCreateSchemaFolder(schemaName, package);
            repository = _intentModelMapper.CreateRepository(repositoryName, schemaFolder.Id);
            package.Classes.Add(repository);
        }

        return repository;
    }

    /// <summary>
    /// Processes foreign keys to create associations and apply FK stereotypes
    /// </summary>
    private void ProcessForeignKeys(DatabaseSchema databaseSchema, PackageModelPersistable package, PackageUpdateResult result)
    {
        if (!_config.ExportTables()) return;

        foreach (var table in databaseSchema.Tables)
        {
            // Find the corresponding class element
            var tableExternalRef = IntentModelMapper.GetTableExternalReference(table.Name, table.Schema);
            var classElement = package.Classes.FirstOrDefault(c => c.ExternalReference == tableExternalRef);

            if (classElement == null) continue;

            // Process each foreign key
            foreach (var foreignKey in table.ForeignKeys)
            {
                // Create association
                var association = _intentModelMapper.GetOrCreateAssociation(foreignKey, table, classElement, package);

                if (association != null)
                {
                    // Apply FK stereotypes to corresponding columns
                    foreach (var fkColumn in foreignKey.Columns)
                    {
                        // Find the corresponding attribute
                        var columnExternalRef = IntentModelMapper.GetColumnExternalReference(fkColumn.Name, table.Name, table.Schema);
                        var attribute = classElement.ChildElements.FirstOrDefault(attr =>
                            attr.ExternalReference == columnExternalRef);

                        if (attribute != null)
                        {
                            // Find the corresponding column schema
                            var columnSchema = table.Columns.FirstOrDefault(c => c.Name == fkColumn.Name);

                            if (columnSchema != null && IntentModelMapper.ShouldApplyForeignKeyStereotype(columnSchema, foreignKey))
                            {
                                // Apply FK stereotype with association link
                                RdbmsSchemaAnnotator.ApplyForeignKey(columnSchema, attribute, association.TargetEnd.Id);
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Processes stored procedure data contract creation for procedures with result sets
    /// Following the pattern from old DatabaseSchemaToModelMapper.GetOrCreateDataContractResponse
    /// </summary>
    private void ProcessStoredProcedureDataContract(
        StoredProcedureSchema storedProc,
        ElementPersistable procElement,
        PackageModelPersistable package,
        List<ElementPersistable> existingElements,
        PackageUpdateResult result,
        DeduplicationContext? deduplicationContext)
    {
        // Get schema folder for data contract placement (same as stored procedure)
        var schemaFolder = GetOrCreateSchemaFolder(storedProc.Schema, package);

        // Check if data contract already exists using ExternalReference first
        var dataContractExternalRef = IntentModelMapper.GetDataContractExternalReference(storedProc.Name, storedProc.Schema);
        var existingDataContract = existingElements.FirstOrDefault(c =>
                                       c.ExternalReference == dataContractExternalRef && c.SpecializationType == "Data Contract") ??
                                   package.Classes.FirstOrDefault(c =>
                                       c.ExternalReference == dataContractExternalRef && c.SpecializationType == "Data Contract");

        ElementPersistable dataContract;
        if (existingDataContract != null)
        {
            // Update existing data contract
            dataContract = _intentModelMapper.CreateDataContractForStoredProcedure(storedProc, schemaFolder.Id, procElement.Name);
            UpdateExistingClass(existingDataContract, dataContract);
            result.UpdatedElements.Add(existingDataContract);
            dataContract = existingDataContract; // Use the existing data contract for TypeReference
        }
        else
        {
            // Create new data contract
            dataContract = _intentModelMapper.CreateDataContractForStoredProcedure(storedProc, schemaFolder.Id, procElement.Name);
            package.Classes.Add(dataContract);
            result.AddedElements.Add(dataContract);
        }

        // Set the stored procedure's TypeReference to point to the data contract
        procElement.TypeReference = new TypeReferencePersistable
        {
            Id = Guid.NewGuid().ToString(),
            TypeId = dataContract.Id, // Point to the actual data contract element ID
            IsNullable = false,
            IsCollection = true, // Stored procedures typically return collections
            Stereotypes = new List<StereotypePersistable>(),
            GenericTypeParameters = new List<TypeReferencePersistable>()
        };
    }
}

public class PackageUpdateResult
{
    public bool IsSuccessful { get; set; }
    public string Message { get; set; } = string.Empty;
    public Exception? Exception { get; set; }
    public List<ElementPersistable> AddedElements { get; set; } = new();
    public List<ElementPersistable> UpdatedElements { get; set; } = new();
    public List<ElementPersistable> RemovedElements { get; set; } = new();
}