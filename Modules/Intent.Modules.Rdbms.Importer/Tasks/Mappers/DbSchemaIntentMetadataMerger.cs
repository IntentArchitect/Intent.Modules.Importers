using System;
using System.Collections.Generic;
using System.Linq;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.RelationalDbSchemaImporter.Contracts.DbSchema;
using Intent.RelationalDbSchemaImporter.Contracts.Enums;

namespace Intent.Modules.Rdbms.Importer.Tasks.Mappers;

/// <summary>
/// Uses the <see cref="IntentModelMapper"/> to orchestrate the results 
/// from the RelationalDbSchemaImporter and to merge it into the Intent Architect Persistence Model.
/// </summary>
internal class DbSchemaIntentMetadataMerger
{
    private readonly ImportConfiguration _config;

    // Track schema folders to avoid duplicates
    private readonly Dictionary<string, ElementPersistable> _schemaFolders = new();

    public DbSchemaIntentMetadataMerger(ImportConfiguration config)
    {
        _config = config;
    }

    /// <summary>
    /// Merge the <see cref="DatabaseSchema"/> into the <see cref="PackageModelPersistable"/> using the <see cref="IntentModelMapper"/>.
    /// </summary>
    /// <param name="databaseSchema">The <see cref="DatabaseSchema"/> to merge.</param>
    /// <param name="package">The <see cref="PackageModelPersistable"/> to merge into.</param>
    /// <param name="deduplicationContext">The <see cref="DeduplicationContext"/> to use for deduplication.</param>
    /// <returns>A <see cref="PackageUpdateResult"/> containing the results of the merge.</returns>
    public PackageUpdateResult MergeSchemaAndPackage(DatabaseSchema databaseSchema, PackageModelPersistable package, DeduplicationContext? deduplicationContext = null)
    {
        var result = new PackageUpdateResult();

        try
        {
            if (_config.ExportTables())
            {
                ProcessTables(databaseSchema, package, deduplicationContext, result);
            }
            if (_config.ExportViews())
            {
                ProcessViews(databaseSchema, package, deduplicationContext, result);
            }
            if (_config.ExportStoredProcedures())
            {
                ProcessStoredProcedures(databaseSchema, package, deduplicationContext, result);
            }
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

    private void ProcessTables(
        DatabaseSchema databaseSchema, 
        PackageModelPersistable package, 
        DeduplicationContext? deduplicationContext, 
        PackageUpdateResult result)
    {
        foreach (var table in databaseSchema.Tables)
        {
            // Check if class already exists first (before creating folder structure)
            // Use ExternalReference for robust lookup first, then fallback to name-based
            var tableExternalRef = ModelNamingUtilities.GetTableExternalReference(table.Schema, table.Name);
            var existingClass = package.Classes.FirstOrDefault(c => c.ExternalReference == tableExternalRef);

            if (existingClass is not null)
            {
                // Update existing class without moving it (keep existing ParentFolderId)
                // Don't use deduplication context for updates to preserve existing names
                var updatedClassElement = IntentModelMapper.MapTableToClass(table, _config, package, existingClass.ParentFolderId);
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
                var classElement = IntentModelMapper.MapTableToClass(table, _config, package, schemaFolder.Id, deduplicationContext);

                package.Classes.Add(classElement);
                result.AddedElements.Add(classElement);

                // Process indexes for new class
                ProcessTableIndexes(table, classElement, package);
            }
        }
    }
    
    private void ProcessViews(
        DatabaseSchema databaseSchema, 
        PackageModelPersistable package, 
        DeduplicationContext? deduplicationContext, 
        PackageUpdateResult result)
    {
        foreach (var view in databaseSchema.Views)
        {
            // Check if class already exists first (before creating folder structure)
            // Use ExternalReference for robust lookup first, then fallback to name-based
            var viewExternalRef = ModelNamingUtilities.GetViewExternalReference(view.Schema, view.Name);
            var existingClass = package.Classes.FirstOrDefault(c => c.ExternalReference == viewExternalRef);

            if (existingClass != null)
            {
                // Update existing class without moving it (keep existing ParentFolderId)
                // Don't use deduplication context for updates to preserve existing names
                var updatedClassElement = IntentModelMapper.MapViewToClass(view, _config, package, existingClass.ParentFolderId);
                UpdateExistingClass(existingClass, updatedClassElement);
                result.UpdatedElements.Add(existingClass);
            }
            else
            {
                // Only create schema folder for new elements
                var schemaFolder = GetOrCreateSchemaFolder(view.Schema, package);
                // Use deduplication context for new elements only
                var classElement = IntentModelMapper.MapViewToClass(view, _config, package, schemaFolder.Id, deduplicationContext);

                package.Classes.Add(classElement);
                result.AddedElements.Add(classElement);
            }
        }
    }
    
    private void ProcessStoredProcedures(DatabaseSchema databaseSchema,
        PackageModelPersistable package,
        DeduplicationContext? deduplicationContext,
        PackageUpdateResult result)
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
                procElement = IntentModelMapper.MapStoredProcedureToElement(storedProc, repositoryElement.Id, package, deduplicationContext);
                repositoryElement.ChildElements.Add(procElement);

                // Apply stored procedure stereotypes
                RdbmsSchemaAnnotator.ApplyStoredProcedureElementSettings(storedProc, procElement);

                result.AddedElements.Add(procElement);
            }
            else
            {
                // Create repository first if it doesn't exist
                var repositoryElement = GetOrCreateRepository(_config.RepositoryElementId, storedProc.Schema, package);

                // Create as operation within repository
                procElement = IntentModelMapper.MapStoredProcedureToOperation(storedProc, repositoryElement.Id, package, deduplicationContext);
                repositoryElement.ChildElements.Add(procElement);

                // Apply stored procedure stereotypes
                RdbmsSchemaAnnotator.ApplyStoredProcedureOperationSettings(storedProc, procElement);

                result.AddedElements.Add(procElement);
            }

            // Create data contract if stored procedure has result set
            if (storedProc.ResultSetColumns.Count > 0)
            {
                ProcessStoredProcedureDataContract(storedProc, procElement, package, result, deduplicationContext);
            }
        }
    }

    /// <summary>
    /// Processes table indexes and creates them in the package
    /// </summary>
    private static void ProcessTableIndexes(TableSchema table, ElementPersistable classElement, PackageModelPersistable package)
    {
        foreach (var index in table.Indexes)
        {
            var indexExternalRef = ModelNamingUtilities.GetIndexExternalReference(table.Schema, table.Name, index.Name);
            var indexElement = package.Classes
                                   .FirstOrDefault(x => x.ExternalReference == indexExternalRef &&
                                                        x.SpecializationType == Constants.SpecializationTypes.Index.SpecializationType)
                               ?? classElement.ChildElements
                                   .FirstOrDefault(x => x.ExternalReference == indexExternalRef &&
                                                        x.SpecializationType == Constants.SpecializationTypes.Index.SpecializationType);
            if (indexElement is null)
            {
                indexElement = IntentModelMapper.CreateIndex(table, index, classElement.Id, package);
                package.Classes.Add(indexElement);
            }

            // Create index columns
            foreach (var indexColumn in index.Columns)
            {
                // Find the corresponding attribute in the class
                var attribute = classElement.ChildElements.FirstOrDefault(attr =>
                    attr.Name.Equals(indexColumn.Name, StringComparison.OrdinalIgnoreCase));

                var indexColumnElement = IntentModelMapper.CreateIndexColumn(indexColumn, indexElement.Id, attribute?.Id, package);
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
            c.SpecializationType == Constants.SpecializationTypes.Folder.SpecializationType);

        if (folder == null)
        {
            folder = IntentModelMapper.CreateSchemaFolder(schemaName, package);
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
        var repositoryName = $"StoredProcedureRepository";

        // Check if repository already exists
        var repository = !string.IsNullOrWhiteSpace(repositoryElementId)
            ? package.Classes.FirstOrDefault(x => x.Id == repositoryElementId) 
              ?? throw new Exception($"Selected Repository could not be found. Did you save your designer before running the importer?")
            : package.Classes.FirstOrDefault(c => c.Name == repositoryName && c.SpecializationType == Constants.SpecializationTypes.Repository.SpecializationType);

        if (repository == null)
        {
            // Get or create schema folder for repository organization
            var schemaFolder = GetOrCreateSchemaFolder(schemaName, package);
            repository = IntentModelMapper.CreateRepository(repositoryName, schemaFolder.Id, package);
            package.Classes.Add(repository);
        }

        return repository;
    }

    /// <summary>
    /// Processes foreign keys to create associations and apply FK stereotypes
    /// </summary>
    private void ProcessForeignKeys(DatabaseSchema databaseSchema, PackageModelPersistable package, PackageUpdateResult result)
    {
        if (!_config.ExportTables())
        {
            return;
        }

        foreach (var table in databaseSchema.Tables)
        {
            var tableExternalRef = ModelNamingUtilities.GetTableExternalReference(table.Schema, table.Name);
            var classElement = package.Classes.FirstOrDefault(c => c.ExternalReference == tableExternalRef);

            if (classElement == null)
            {
                continue;
            }

            foreach (var foreignKey in table.ForeignKeys)
            {
                var association = IntentModelMapper.GetOrCreateAssociation(foreignKey, table, classElement, package);
                if (association == null)
                {
                    continue;
                }
                
                foreach (var fkColumn in foreignKey.Columns)
                {
                    var columnExternalRef = ModelNamingUtilities.GetColumnExternalReference(table.Schema, table.Name, fkColumn.Name);
                    var attribute = classElement.ChildElements.FirstOrDefault(attr =>
                        attr.ExternalReference == columnExternalRef);

                    if (attribute == null)
                    {
                        continue;
                    }
                    var columnSchema = table.Columns.FirstOrDefault(c => c.Name == fkColumn.Name);
                    if (columnSchema != null && ShouldApplyForeignKeyStereotype(columnSchema, foreignKey))
                    {
                        RdbmsSchemaAnnotator.ApplyForeignKey(columnSchema, attribute, association.TargetEnd.Id);
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// Determines if a column should get a Foreign Key stereotype
    /// Excludes columns that are both Primary Key and Foreign Key (one-to-one relationships)
    /// </summary>
    private static bool ShouldApplyForeignKeyStereotype(ColumnSchema column, ForeignKeySchema foreignKey)
    {
        // Don't apply FK stereotype if the column is both PK and FK (one-to-one relationship)
        return !column.IsPrimaryKey || foreignKey.Columns.All(fk => fk.Name != column.Name);
    }

    /// <summary>
    /// Processes stored procedure data contract creation for procedures with result sets
    /// Following the pattern from old DatabaseSchemaToModelMapper.GetOrCreateDataContractResponse
    /// </summary>
    private void ProcessStoredProcedureDataContract(
        StoredProcedureSchema storedProc,
        ElementPersistable procElement,
        PackageModelPersistable package,
        PackageUpdateResult result,
        DeduplicationContext? deduplicationContext)
    {
        // Get schema folder for data contract placement (same as stored procedure)
        var schemaFolder = GetOrCreateSchemaFolder(storedProc.Schema, package);

        // Check if data contract already exists using ExternalReference first
        var dataContractExternalRef = ModelNamingUtilities.GetDataContractExternalReference(storedProc.Schema, storedProc.Name);
        var existingDataContract = package.Classes.FirstOrDefault(c => c.ExternalReference == dataContractExternalRef && 
                                                                          c.SpecializationType == Constants.SpecializationTypes.DataContract.SpecializationType);

        ElementPersistable dataContract;
        if (existingDataContract != null)
        {
            // Update existing data contract
            dataContract = IntentModelMapper.CreateDataContractForStoredProcedure(storedProc, schemaFolder.Id, procElement.Name, package);
            UpdateExistingClass(existingDataContract, dataContract);
            result.UpdatedElements.Add(existingDataContract);
            dataContract = existingDataContract; // Use the existing data contract for TypeReference
        }
        else
        {
            // Create new data contract
            dataContract = IntentModelMapper.CreateDataContractForStoredProcedure(storedProc, schemaFolder.Id, procElement.Name, package);
            package.Classes.Add(dataContract);
            result.AddedElements.Add(dataContract);
        }

        procElement.TypeReference.TypeId = dataContract.Id; // Point to the data contract element ID
        procElement.TypeReference.IsCollection = true; // Stored procedures typically return collections
    }
}

internal class PackageUpdateResult
{
    public bool IsSuccessful { get; set; }
    public string Message { get; set; } = string.Empty;
    public Exception? Exception { get; set; }
    public List<ElementPersistable> AddedElements { get; set; } = new();
    public List<ElementPersistable> UpdatedElements { get; set; } = new();
    public List<ElementPersistable> RemovedElements { get; set; } = new();
}