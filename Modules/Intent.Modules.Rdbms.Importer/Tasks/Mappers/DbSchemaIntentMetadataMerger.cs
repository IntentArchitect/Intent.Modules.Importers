using System;
using System.Collections.Generic;
using System.Linq;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Modelers.Domain.Api;
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
            // Use unified lookup helper with 3-level precedence
            var tableExternalRef = ModelNamingUtilities.GetTableExternalReference(table.Schema, table.Name);
            var className = ModelNamingUtilities.GetEntityName(table.Name, _config.EntityNameConvention, table.Schema, null);
            
            var existingClass = IntentModelMapper.FindElementWithPrecedence(
                package.Classes,
                tableExternalRef,
                className,
                table.Schema,
                ClassModel.SpecializationType);

            if (existingClass is not null)
            {
                // Update existing class without moving it (keep existing ParentFolderId)
                // Don't use deduplication context for updates to preserve existing names
                var updatedClassElement = IntentModelMapper.MapTableToClass(table, _config, package, existingClass.ParentFolderId);
                SyncElements(package, existingClass, updatedClassElement);
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
            // Use unified lookup helper with 3-level precedence
            var viewExternalRef = ModelNamingUtilities.GetViewExternalReference(view.Schema, view.Name);
            var className = ModelNamingUtilities.GetViewName(view.Name, _config.EntityNameConvention, view.Schema, null);
            
            var existingClass = IntentModelMapper.FindElementWithPrecedence(
                package.Classes,
                viewExternalRef,
                className,
                view.Schema,
                ClassModel.SpecializationType);

            if (existingClass != null)
            {
                // Update existing class without moving it (keep existing ParentFolderId)
                // Don't use deduplication context for updates to preserve existing names
                var updatedClassElement = IntentModelMapper.MapViewToClass(view, _config, package, existingClass.ParentFolderId);
                SyncElements(package, existingClass, updatedClassElement);
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
        // First pass: Create UserDefinedTable DataContracts for deduplication
        var udtDataContracts = ProcessUserDefinedTableDataContracts(databaseSchema, package, result, deduplicationContext);

        foreach (var storedProc in databaseSchema.StoredProcedures)
        {
            // Create repository first if it doesn't exist
            var repositoryElement = GetOrCreateRepository(_config.RepositoryElementId, package);

            // Check if stored procedure already exists
            var spExternalRef = ModelNamingUtilities.GetStoredProcedureExternalReference(storedProc.Schema, storedProc.Name);
            var expectedSpecializationType = _config.StoredProcedureType == StoredProcedureType.StoredProcedureElement 
                ? Constants.SpecializationTypes.StoredProcedure.SpecializationType 
                : Constants.SpecializationTypes.Operation.SpecializationType;

            var existingElement = repositoryElement.ChildElements.FirstOrDefault(c => 
                c.ExternalReference == spExternalRef && 
                c.SpecializationType == expectedSpecializationType);

            ElementPersistable procElement;

            if (existingElement != null)
            {
                // Update existing stored procedure element
                if (_config.StoredProcedureType == StoredProcedureType.StoredProcedureElement)
                {
                    var updatedElement = IntentModelMapper.MapStoredProcedureToElement(storedProc, repositoryElement.Id, package, null, udtDataContracts);
                    SyncElements(package, existingElement, updatedElement);
                    RdbmsSchemaAnnotator.ApplyStoredProcedureElementSettings(storedProc, existingElement);
                }
                else
                {
                    var updatedElement = IntentModelMapper.MapStoredProcedureToOperation(storedProc, repositoryElement.Id, package, null, udtDataContracts);
                    SyncElements(package, existingElement, updatedElement);
                    RdbmsSchemaAnnotator.ApplyStoredProcedureOperationSettings(storedProc, existingElement);
                }
                
                procElement = existingElement;
                result.UpdatedElements.Add(existingElement);
            }
            else
            {
                // Create new stored procedure element
                if (_config.StoredProcedureType == StoredProcedureType.StoredProcedureElement)
                {
                    procElement = IntentModelMapper.MapStoredProcedureToElement(storedProc, repositoryElement.Id, package, deduplicationContext, udtDataContracts);
                    RdbmsSchemaAnnotator.ApplyStoredProcedureElementSettings(storedProc, procElement);
                }
                else
                {
                    procElement = IntentModelMapper.MapStoredProcedureToOperation(storedProc, repositoryElement.Id, package, deduplicationContext, udtDataContracts);
                    RdbmsSchemaAnnotator.ApplyStoredProcedureOperationSettings(storedProc, procElement);
                }
                
                repositoryElement.ChildElements.Add(procElement);
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
                var attrColumnExternalRef = ModelNamingUtilities.GetColumnExternalReference(table.Schema, table.Name, indexColumn.Name);
                var attribute = classElement.ChildElements.FirstOrDefault(attr => attr.ExternalReference == attrColumnExternalRef && attr.SpecializationType == AttributeModel.SpecializationType);

                var indexColumnElement = indexElement.ChildElements
                    .FirstOrDefault(x => (x.ExternalReference == ModelNamingUtilities.GetIndexColumnExternalReference(indexColumn.Name) && x.SpecializationType == Constants.SpecializationTypes.IndexColumn.SpecializationType)
                                         || (x.Name == indexColumn.Name && x.SpecializationType == Constants.SpecializationTypes.IndexColumn.SpecializationType));
                if (indexColumnElement is null)
                {
                    indexColumnElement = IntentModelMapper.CreateIndexColumn(indexColumn, indexElement.Id, attribute?.Id, package);
                    indexElement.ChildElements.Add(indexColumnElement);
                }
            }
        }
    }

    /// <summary>
    /// Synchronizes properties between existing and source elements using add-or-modify approach.
    /// Overwrites TypeReference, syncs Stereotypes and ChildElements without destructive replacement.
    /// </summary>
    /// <param name="package">Container package</param>
    /// <param name="existingElement">The existing element to be updated</param>
    /// <param name="sourceElement">The source element containing new/updated data</param>
    private static void SyncElements(PackageModelPersistable package, ElementPersistable existingElement, ElementPersistable sourceElement)
    {
        // Extract parent element's schema for child element lookups
        var parentSchema = IntentModelMapper.GetElementDbSchema(existingElement);
        
        InternSyncElements(
            package: package,
            existingElement: existingElement,
            sourceElement: sourceElement,
            parentSchema: parentSchema,
            visitedElements: new HashSet<ElementPersistable>(EqualityComparer<ElementPersistable>.Create(
                (a, b) =>
                    (
                        (
                            (a?.ExternalReference is null || b?.ExternalReference is null) &&
                            string.Equals(a?.Name, b?.Name, StringComparison.InvariantCultureIgnoreCase)
                        )
                        ||
                        (
                            a?.ExternalReference is not null && b?.ExternalReference is not null &&
                            string.Equals(a.ExternalReference, b.ExternalReference, StringComparison.InvariantCultureIgnoreCase)
                        )
                    ) 
                    && a?.SpecializationType == b?.SpecializationType,
                x => x.ExternalReference?.GetHashCode() ?? x.Name?.GetHashCode() ?? 0)));
        return;
        
        static void InternSyncElements(
            PackageModelPersistable package,
            ElementPersistable existingElement, 
            ElementPersistable sourceElement,
            string? parentSchema,
            HashSet<ElementPersistable> visitedElements)
        {
            // Update type reference (existing behavior - direct overwrite)
            if (sourceElement.TypeReference != null)
            {
                existingElement.TypeReference ??= new TypeReferencePersistable();
                existingElement.TypeReference.TypeId = sourceElement.TypeReference.TypeId;
                existingElement.TypeReference.GenericTypeId = sourceElement.TypeReference.GenericTypeId;
                existingElement.TypeReference.IsCollection = sourceElement.TypeReference.IsCollection;
                existingElement.TypeReference.IsNullable = sourceElement.TypeReference.IsNullable;
                SyncStereotypeCollection(existingElement.Stereotypes, sourceElement.Stereotypes);
            }

            if (string.IsNullOrWhiteSpace(existingElement.ExternalReference))
            {
                existingElement.ExternalReference = sourceElement.ExternalReference;
            }

            // Sync stereotypes using the add-or-modify approach
            existingElement.Stereotypes ??= [];
            if (sourceElement.Stereotypes.Count > 0)
            {
                SyncStereotypeCollection(existingElement.Stereotypes, sourceElement.Stereotypes);
            }
            

            // Sync child elements using the unified lookup helper with 3-level precedence
            existingElement.ChildElements ??= [];
            if (sourceElement.ChildElements.Count > 0)
            {
                foreach (var sourceChild in sourceElement.ChildElements)
                {
                    // Use unified lookup helper for child elements with 3-level precedence
                    // 1. ExternalReference + SpecializationType
                    // 2. Name + Parent Schema + SpecializationType  
                    // 3. Name + SpecializationType
                    var existingChild = IntentModelMapper.FindElementWithPrecedence(
                        existingElement.ChildElements,
                        sourceChild.ExternalReference,
                        sourceChild.Name,
                        parentSchema, // Use parent element's schema for context
                        sourceChild.SpecializationType);
                
                    if (existingChild is null)
                    {
                        // Add a new child element
                        existingElement.ChildElements.Add(sourceChild);
                    }
                    else if (visitedElements.Add(sourceChild))
                    {
                        // Extract child's schema for recursive calls
                        var childSchema = IntentModelMapper.GetElementDbSchema(existingChild) ?? parentSchema;
                        
                        // Recursively sync the child element
                        InternSyncElements(package, existingChild, sourceChild, childSchema, visitedElements);
                    }
                }
            }
        }
    }

    private static void SyncStereotypeCollection(List<StereotypePersistable> existingElementStereotypes, List<StereotypePersistable> sourceElementStereotypes)
    {
        foreach (var sourceStereotype in sourceElementStereotypes)
        {
            var existingStereotype = existingElementStereotypes
                .FirstOrDefault(s => s.DefinitionId == sourceStereotype.DefinitionId);
                
            if (existingStereotype is null)
            {
                // Add new stereotype
                existingElementStereotypes.Add(sourceStereotype);
            }
            else
            {
                // Sync properties within existing stereotype
                SyncStereotypeProperties(existingStereotype, sourceStereotype);
            }
        }
    }

    /// <summary>
    /// Synchronizes properties between existing and source stereotypes using add-or-modify approach.
    /// </summary>
    /// <param name="existingStereotype">The existing stereotype to be updated</param>
    /// <param name="sourceStereotype">The source stereotype containing new/updated properties</param>
    private static void SyncStereotypeProperties(StereotypePersistable existingStereotype, StereotypePersistable sourceStereotype)
    {
        existingStereotype.Properties ??= [];
        
        if (sourceStereotype.Properties.Count == 0)
        {
            return;
        }
        
        foreach (var sourceProperty in sourceStereotype.Properties)
        {
            var existingProperty = existingStereotype.Properties
                .FirstOrDefault(p => p.DefinitionId == sourceProperty.DefinitionId);
            
            if (existingProperty == null)
            {
                // Add new property
                existingStereotype.Properties.Add(sourceProperty);
            }
            else
            {
                // Update existing property values
                existingProperty.Value = sourceProperty.Value;
            }
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
            c.Name == ModelNamingUtilities.NormalizeSchemaName(schemaName) &&
            c.SpecializationType == Constants.SpecializationTypes.Folder.SpecializationType);

        if (folder == null)
        {
            folder = IntentModelMapper.CreateSchemaFolder(schemaName, package);
            package.Classes.Add(folder);
        }

        _schemaFolders[schemaName] = folder;
        return folder;
    }

    /// <summary>
    /// Gets or creates a repository element for stored procedure operations
    /// </summary>
    private ElementPersistable GetOrCreateRepository(string? repositoryElementId, PackageModelPersistable package)
    {
        var repositoryName = "StoredProcedureRepository";

        // Check if repository already exists
        var repository = !string.IsNullOrWhiteSpace(repositoryElementId)
            ? package.Classes.FirstOrDefault(x => x.Id == repositoryElementId) 
              ?? throw new Exception("Selected Repository could not be found. Did you save your designer before running the importer?")
            : package.Classes.FirstOrDefault(c => c.Name == repositoryName && c.SpecializationType == Constants.SpecializationTypes.Repository.SpecializationType);

        if (repository == null)
        {
            // Create repository at package level (not inside schema folders)
            repository = IntentModelMapper.CreateRepository(repositoryName, package.Id, package);
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
            var classElement = package.Classes.FirstOrDefault(c => c.ExternalReference == tableExternalRef && c.SpecializationType == ClassModel.SpecializationType);

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
                    if (columnSchema != null)
                    {
                        RdbmsSchemaAnnotator.ApplyForeignKey(columnSchema, attribute, association.TargetEnd?.Id);
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
        PackageUpdateResult result,
        DeduplicationContext? deduplicationContext)
    {
        // Get schema folder for data contract placement (same as stored procedure)
        var schemaFolder = GetOrCreateSchemaFolder(storedProc.Schema, package);

        // Use unified lookup helper with 3-level precedence
        var dataContractExternalRef = ModelNamingUtilities.GetDataContractExternalReference(storedProc.Schema, storedProc.Name);
        var dataContractName = $"{procElement.Name}Response";
        
        var existingDataContract = IntentModelMapper.FindElementWithPrecedence(
            package.Classes,
            dataContractExternalRef,
            dataContractName,
            storedProc.Schema,
            Constants.SpecializationTypes.DataContract.SpecializationType);

        ElementPersistable dataContract;
        if (existingDataContract != null)
        {
            // Update existing data contract
            dataContract = IntentModelMapper.CreateDataContractForStoredProcedure(storedProc, schemaFolder.Id, procElement.Name, package);
            SyncElements(package, existingDataContract, dataContract);
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

    /// <summary>
    /// Processes UserDefinedTable parameters from stored procedures and creates corresponding DataContracts
    /// Returns a dictionary mapping UDT external references to their DataContract IDs for deduplication
    /// </summary>
    private Dictionary<string, string> ProcessUserDefinedTableDataContracts(
        DatabaseSchema databaseSchema,
        PackageModelPersistable package,
        PackageUpdateResult result,
        DeduplicationContext? deduplicationContext)
    {
        var udtDataContracts = new Dictionary<string, string>(); // External ref -> DataContract ID

        // Collect all unique UserDefinedTables from stored procedure parameters
        var uniqueUdts = new Dictionary<string, UserDefinedTableTypeSchema>();
        
        foreach (var storedProc in databaseSchema.StoredProcedures)
        {
            foreach (var parameter in storedProc.Parameters)
            {
                if (parameter.UserDefinedTableType != null)
                {
                    var udtExternalRef = ModelNamingUtilities.GetUserDefinedTableDataContractExternalReference(
                        parameter.UserDefinedTableType.Schema, 
                        parameter.UserDefinedTableType.Name);
                    
                    if (!uniqueUdts.ContainsKey(udtExternalRef))
                    {
                        uniqueUdts[udtExternalRef] = parameter.UserDefinedTableType;
                    }
                }
            }
        }

        // Create DataContracts for each unique UDT
        foreach (var kvp in uniqueUdts)
        {
            var udtExternalRef = kvp.Key;
            var udtSchema = kvp.Value;

            // Use unified lookup helper with 3-level precedence
            var dataContractName = ModelNamingUtilities.NormalizeUserDefinedTableName(udtSchema.Name);
            
            var existingDataContract = IntentModelMapper.FindElementWithPrecedence(
                package.Classes,
                udtExternalRef,
                dataContractName,
                udtSchema.Schema,
                Constants.SpecializationTypes.DataContract.SpecializationType);

            ElementPersistable dataContract;
            if (existingDataContract != null)
            {
                // Update existing DataContract
                var schemaFolder = GetOrCreateSchemaFolder(udtSchema.Schema, package);
                dataContract = IntentModelMapper.CreateDataContractForUserDefinedTable(udtSchema, schemaFolder.Id, package);
                SyncElements(package, existingDataContract, dataContract);
                result.UpdatedElements.Add(existingDataContract);
                dataContract = existingDataContract; // Use existing for mapping
            }
            else
            {
                // Create new DataContract
                var schemaFolder = GetOrCreateSchemaFolder(udtSchema.Schema, package);
                dataContract = IntentModelMapper.CreateDataContractForUserDefinedTable(udtSchema, schemaFolder.Id, package);
                package.Classes.Add(dataContract);
                result.AddedElements.Add(dataContract);
            }

            udtDataContracts[udtExternalRef] = dataContract.Id;
        }

        return udtDataContracts;
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