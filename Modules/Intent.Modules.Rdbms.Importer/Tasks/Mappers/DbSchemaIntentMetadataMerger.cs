using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// <returns>A <see cref="MergeResult"/> containing the results of the merge.</returns>
    public MergeResult MergeSchemaAndPackage(DatabaseSchema databaseSchema, PackageModelPersistable package, DeduplicationContext? deduplicationContext = null)
    {
        var result = new MergeResult();

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
        MergeResult result)
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
                ClassModel.SpecializationType,
                package);

            if (existingClass is not null)
            {
                // Update existing class without moving it (keep existing ParentFolderId)
                // Don't use deduplication context for updates to preserve existing names
                var updatedClassElement = IntentModelMapper.MapTableToClass(table, _config, package, existingClass.ParentFolderId);
                SyncElements(package, existingClass, updatedClassElement, _config.AllowDeletions, _config.PreserveAttributeTypes, result);

                // Re-evaluate stereotypes on existing class after sync to ensure renamed attributes get proper Column stereotypes
                ApplyTableStereotypes(table, existingClass, _config);

                // Process indexes for existing class
                ProcessTableIndexes(table, existingClass, package, result);
            }
            else
            {
                // Only create schema folder for new elements
                var schemaFolder = GetOrCreateSchemaFolder(table.Schema, package);
                // Use deduplication context for new elements only
                var classElement = IntentModelMapper.MapTableToClass(table, _config, package, schemaFolder.Id, deduplicationContext);

                package.Classes.Add(classElement);

                // Re-evaluate stereotypes on existing class after sync to ensure renamed attributes get proper Column stereotypes
                ApplyTableStereotypes(table, classElement, _config);
                
                // Process indexes for new class
                ProcessTableIndexes(table, classElement, package, result);
            }
        }
    }
    
    private void ProcessViews(
        DatabaseSchema databaseSchema, 
        PackageModelPersistable package, 
        DeduplicationContext? deduplicationContext, 
        MergeResult result)
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
                ClassModel.SpecializationType,
                package);

            if (existingClass != null)
            {
                // Update existing class without moving it (keep existing ParentFolderId)
                // Don't use deduplication context for updates to preserve existing names
                var updatedClassElement = IntentModelMapper.MapViewToClass(view, _config, package, existingClass.ParentFolderId);
                SyncElements(package, existingClass, updatedClassElement, _config.AllowDeletions, _config.PreserveAttributeTypes, result);
                
                // Apply view stereotypes after sync
                ApplyViewStereotypes(view, existingClass);
            }
            else
            {
                // Only create schema folder for new elements
                var schemaFolder = GetOrCreateSchemaFolder(view.Schema, package);
                // Use deduplication context for new elements only
                var classElement = IntentModelMapper.MapViewToClass(view, _config, package, schemaFolder.Id, deduplicationContext);

                package.Classes.Add(classElement);
                
                // Apply view stereotypes for new elements
                ApplyViewStereotypes(view, classElement);
            }
        }
    }
    
    private void ProcessStoredProcedures(DatabaseSchema databaseSchema,
        PackageModelPersistable package,
        DeduplicationContext? deduplicationContext,
        MergeResult result)
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
                    SyncElements(package, existingElement, updatedElement, _config.AllowDeletions, _config.PreserveAttributeTypes, result);
                    RdbmsSchemaAnnotator.ApplyStoredProcedureElementSettings(storedProc, existingElement);
                }
                else
                {
                    var updatedElement = IntentModelMapper.MapStoredProcedureToOperation(storedProc, repositoryElement.Id, package, null, udtDataContracts);
                    SyncElements(package, existingElement, updatedElement, _config.AllowDeletions, _config.PreserveAttributeTypes, result);
                    RdbmsSchemaAnnotator.ApplyStoredProcedureOperationSettings(storedProc, existingElement);
                }
                
                procElement = existingElement;
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
            }

            if (storedProc.ResultSetColumns.Count > 0)
            {
                ProcessStoredProcedureDataContract(storedProc, procElement, package, result, deduplicationContext);
            }
        }
    }

    /// <summary>
    /// Processes table indexes and creates them in the package
    /// </summary>
    private static void ProcessTableIndexes(TableSchema table, ElementPersistable classElement, PackageModelPersistable package, MergeResult result)
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
            
            RdbmsSchemaAnnotator.ApplyIndexStereotype(indexElement, index);

            // Create index columns
            foreach (var indexColumn in index.Columns)
            {
                // Find the corresponding attribute in the class
                var attrColumnExternalRef = ModelNamingUtilities.GetColumnExternalReference(table.Schema, table.Name, indexColumn.Name);
                var attribute = classElement.ChildElements.FirstOrDefault(attr => attr.ExternalReference == attrColumnExternalRef && attr.SpecializationType == AttributeModel.SpecializationType);

                if (attribute is null)
                {
                    result.Warnings.Add($"Index column '{indexColumn.Name}' in index '{index.Name}' could not be mapped to an attribute in table '{table.Name}'.");
                    continue;
                }
                
                var indexColumnElement = indexElement.ChildElements
                    .FirstOrDefault(x => (x.ExternalReference == ModelNamingUtilities.GetIndexColumnExternalReference(indexColumn.Name) && x.SpecializationType == Constants.SpecializationTypes.IndexColumn.SpecializationType)
                                         || (x.Name == indexColumn.Name && x.SpecializationType == Constants.SpecializationTypes.IndexColumn.SpecializationType));
                if (indexColumnElement is null)
                {
                    indexColumnElement = IntentModelMapper.CreateIndexColumn(indexColumn, indexElement.Id, attribute.Id, package);
                    indexElement.ChildElements.Add(indexColumnElement);
                }
                
                RdbmsSchemaAnnotator.ApplyIndexColumnStereotype(indexColumnElement, indexColumn);
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
    /// <param name="allowDeletions">Whether to remove child elements with external references that don't exist in source</param>
    /// <param name="preserveAttributeTypes">Whether to preserve existing attribute type references</param>
    /// <param name="result">Optional merge result to collect warnings about deleted elements</param>
    private static void SyncElements(
        PackageModelPersistable package, 
        ElementPersistable existingElement, 
        ElementPersistable sourceElement,
        bool allowDeletions = false,
        bool preserveAttributeTypes = false,
        MergeResult? result = null)
    {
        // Extract parent element's schema for child element lookups
        var parentSchema = IntentModelMapper.GetElementDbSchema(existingElement);
        
        InternSyncElements(
            package: package,
            existingElement: existingElement,
            sourceElement: sourceElement,
            parentSchema: parentSchema,
            allowDeletions: allowDeletions,
            preserveAttributeTypes: preserveAttributeTypes,
            result: result,
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
            bool allowDeletions,
            bool preserveAttributeTypes,
            MergeResult? result,
            HashSet<ElementPersistable> visitedElements)
        {
            // Update type reference (existing behavior - direct overwrite)
            // Only update TypeId if preserveAttributeTypes is false OR if the existing element has no TypeReference
            if (sourceElement.TypeReference != null)
            {
                existingElement.TypeReference ??= new TypeReferencePersistable();
                
                // Only overwrite TypeId if not preserving types or if it's a new element
                if (!preserveAttributeTypes || string.IsNullOrEmpty(existingElement.TypeReference.TypeId))
                {
                    existingElement.TypeReference.TypeId = sourceElement.TypeReference.TypeId;
                }
                // I want a nice notification here
                // else if (preserveAttributeTypes && 
                //          !string.IsNullOrEmpty(existingElement.TypeReference.TypeId) &&
                //          existingElement.TypeReference.TypeId != sourceElement.TypeReference.TypeId)
                // {
                //     // Log warning when type preservation prevents an update
                //     var elementIdentifier = string.IsNullOrEmpty(parentSchema) 
                //         ? existingElement.Name 
                //         : $"{parentSchema}.{existingElement.Name}";
                //     result?.Warnings.Add($"Preserved user-specified type for '{elementIdentifier}'. Database type would have changed it to '{sourceElement.TypeReference.TypeName}'.");
                // }
                
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
            
            // Track source external references for deletion detection
            var sourceExternalRefs = new HashSet<string>(
                sourceElement.ChildElements
                    .Where(c => !string.IsNullOrWhiteSpace(c.ExternalReference))
                    .Select(c => c.ExternalReference!),
                StringComparer.OrdinalIgnoreCase);
            
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
                    sourceChild.SpecializationType,
                    package);
                
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
                    InternSyncElements(package, existingChild, sourceChild, childSchema, allowDeletions, preserveAttributeTypes, result, visitedElements);
                }
            }
            
            // Remove child elements that have external references but don't exist in source
            if (allowDeletions)
            {
                var elementsToRemove = existingElement.ChildElements
                    .Where(existing => 
                        !string.IsNullOrWhiteSpace(existing.ExternalReference) &&
                        existing.SpecializationType == AttributeModel.SpecializationType && // Only delete attributes
                        !sourceExternalRefs.Contains(existing.ExternalReference))
                    .ToList();

                foreach (var elementToRemove in elementsToRemove)
                {
                    existingElement.ChildElements.Remove(elementToRemove);
                    
                    // Log the deletion as a warning
                    result?.Warnings.Add(
                        $"Removed attribute '{elementToRemove.Name}' from '{existingElement.Name}' (no longer exists in database).");
                }
            }
        }
    }

    /// <summary>
    /// Removes associations that reference deleted foreign keys.
    /// The association's ExternalReference matches the FK's external reference from ModelNamingUtilities.GetForeignKeyExternalReference().
    /// This is called after ProcessForeignKeys completes, so we know exactly which FKs exist in the database.
    /// Only removes associations from tables that are being imported to support inclusive imports.
    /// </summary>
    /// <param name="package">The package containing associations</param>
    /// <param name="sourceFkExternalRefs">Set of FK external references that exist in the source database</param>
    /// <param name="importedTableExternalRefs">Set of table external references being imported in this operation</param>
    /// <param name="result">Optional merge result to collect warnings about deleted associations</param>
    private static void RemoveObsoleteAssociations(
        PackageModelPersistable package, 
        HashSet<string> sourceFkExternalRefs,
        HashSet<string> importedTableExternalRefs,
        MergeResult? result)
    {
        if (package.Associations == null || package.Associations.Count == 0)
        {
            return;
        }

        // Find associations with external references that no longer exist in the source database
        // AND where the source class is being imported (to support inclusive imports)
        var associationsToRemove = package.Associations
            .Where(assoc => 
            {
                // Must have an external reference (from a FK)
                if (string.IsNullOrWhiteSpace(assoc.ExternalReference))
                    return false;
                
                // FK must no longer exist in the database
                if (sourceFkExternalRefs.Contains(assoc.ExternalReference))
                    return false;
                
                // Only remove if the source class is being imported
                var sourceClass = package.Classes
                    .FirstOrDefault(c => c.Id == assoc.SourceEnd?.TypeReference?.TypeId);
                
                if (sourceClass == null)
                    return false;
                
                // Only remove associations from tables we're actually importing
                return importedTableExternalRefs.Contains(sourceClass.ExternalReference ?? string.Empty);
            })
            .ToList();

        foreach (var association in associationsToRemove)
        {
            // Get source class for FK stereotype removal
            var sourceClass = package.Classes
                .FirstOrDefault(c => c.Id == association.SourceEnd?.TypeReference?.TypeId);

            // Remove FK stereotypes and metadata from attributes referencing this association
            if (sourceClass != null && association.TargetEnd?.Id != null)
            {
                RdbmsSchemaAnnotator.RemoveForeignKeysForAssociation(sourceClass, association.TargetEnd.Id);
            }

            package.Associations.Remove(association);
            
            result?.Warnings.Add(
                $"Removed association '{association.TargetEnd?.Name}' from '{sourceClass?.Name ?? "Unknown"}' (foreign key no longer exists in database).");
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
            c.Name == ModelNamingUtilities.GetFolderName(schemaName) &&
            c.SpecializationType == Constants.SpecializationTypes.Folder.SpecializationType);

        if (folder == null)
        {
            folder = IntentModelMapper.CreateSchemaFolder(schemaName, package);
            package.Classes.Add(folder);
        }
        
        // Apply schema stereotypes for existing folders that might be missing them
        ApplySchemaFolderStereotypes(schemaName, folder);

        _schemaFolders[schemaName] = folder;
        return folder;
    }

    /// <summary>
    /// Gets or creates a repository element for stored procedure operations
    /// </summary>
    private static ElementPersistable GetOrCreateRepository(string? repositoryElementId, PackageModelPersistable package)
    {
        const string defaultRepositoryName = "StoredProcedureRepository";

        ElementPersistable? repository;
        if (!string.IsNullOrWhiteSpace(repositoryElementId))
        {
            repository = package.Classes.FirstOrDefault(x => x.Id == repositoryElementId)
                         ?? throw new Exception("Selected Repository could not be found. Did you save your designer before running the importer?");
        }
        else
        {
            repository = package.Classes.FirstOrDefault(x => x.ExternalReference == ModelNamingUtilities.GetStoredProcedureRepositoryExternalReference())
                ?? package.Classes.FirstOrDefault(c => c.Name == defaultRepositoryName && c.SpecializationType == Constants.SpecializationTypes.Repository.SpecializationType);
        }

        if (repository == null)
        {
            // Create repository at package level (not inside schema folders)
            repository = IntentModelMapper.CreateRepository(defaultRepositoryName, package.Id, package, ModelNamingUtilities.GetStoredProcedureRepositoryExternalReference());
            package.Classes.Add(repository);
        }
        else if (string.IsNullOrWhiteSpace(repository.ExternalReference))
        {
            repository.ExternalReference = ModelNamingUtilities.GetStoredProcedureRepositoryExternalReference();
        }

        return repository;
    }

    /// <summary>
    /// Processes foreign keys to create associations and apply FK stereotypes
    /// </summary>
    private void ProcessForeignKeys(DatabaseSchema databaseSchema, PackageModelPersistable package, MergeResult result)
    {
        if (!_config.ExportTables())
        {
            return;
        }

        // Build a set of all FK external references from the database schema
        var sourceFkExternalRefs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        
        // Track which tables are being imported in this operation
        var importedTableExternalRefs = new HashSet<string>(
            databaseSchema.Tables.Select(t => 
                ModelNamingUtilities.GetTableExternalReference(t.Schema, t.Name)),
            StringComparer.OrdinalIgnoreCase);

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
                // Track this FK as existing in the source database
                var fkExternalRef = ModelNamingUtilities.GetForeignKeyExternalReference(table.Schema, table.Name, foreignKey.Name);
                sourceFkExternalRefs.Add(fkExternalRef);

                var associationResult = IntentModelMapper.GetOrCreateAssociation(foreignKey, table, classElement, package, databaseSchema);
                
                switch (associationResult.Status)
                {
                    case AssociationCreationStatus.Success:
                        // Association was created successfully, continue to apply FK stereotypes
                        break;
                        
                    case AssociationCreationStatus.TargetClassNotFound:
                        result.Warnings.Add(associationResult.Reason!);
                        continue;
                        
                    case AssociationCreationStatus.DuplicateSkipped:
                        result.Warnings.Add(associationResult.Reason!);
                        continue;
                        
                    case AssociationCreationStatus.UnsupportedForeignKey:
                        result.Warnings.Add(associationResult.Reason!);
                        continue;
                        
                    default:
                        throw new InvalidOperationException("Unexpected association creation status: " + associationResult.Status);
                }

                foreach (var fkColumn in foreignKey.Columns)
                {
                    var columnExternalRef = ModelNamingUtilities.GetColumnExternalReference(table.Schema, table.Name, fkColumn.Name);
                    var attribute = classElement.ChildElements.FirstOrDefault(attr =>
                        attr.ExternalReference == columnExternalRef);

                    if (attribute == null)
                    {
                        result.Warnings.Add($"Could not find attribute '{fkColumn.Name}' in table '{table.Name}' for foreign key association.");
                        continue;
                    }
                    var columnSchema = table.Columns.FirstOrDefault(c => c.Name == fkColumn.Name);
                    if (columnSchema != null)
                    {
                        RdbmsSchemaAnnotator.ApplyForeignKey(columnSchema, attribute, associationResult.Association!.TargetEnd?.Id);
                    }
                }
            }
        }

        // Remove obsolete associations after processing all foreign keys
        if (_config.AllowDeletions)
        {
            RemoveObsoleteAssociations(package, sourceFkExternalRefs, importedTableExternalRefs, result);
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
        MergeResult result,
        DeduplicationContext? deduplicationContext)
    {
        if (storedProc.ResultSetColumns.Count > 0)
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
                Constants.SpecializationTypes.DataContract.SpecializationType,
                package);

            ElementPersistable dataContract;
            if (existingDataContract != null)
            {
                // Update existing data contract
                dataContract = IntentModelMapper.CreateDataContractForStoredProcedure(storedProc, schemaFolder.Id, procElement.Name, _config, package, deduplicationContext);
                SyncElements(package, existingDataContract, dataContract, _config.AllowDeletions, _config.PreserveAttributeTypes, result);
                dataContract = existingDataContract; // Use the existing data contract for TypeReference
            }
            else
            {
                // Create new data contract
                dataContract = IntentModelMapper.CreateDataContractForStoredProcedure(storedProc, schemaFolder.Id, procElement.Name, _config, package, deduplicationContext);
                package.Classes.Add(dataContract);
            }
            
            // Apply data contract stereotypes after sync
            ApplyDataContractStereotypes(storedProc, dataContract);

            procElement.TypeReference.TypeId = dataContract.Id; // Point to the data contract element ID
            procElement.TypeReference.IsCollection = true; // Result sets are collections
            procElement.TypeReference.IsNullable = false; // Collections themselves are typically not nullable
        }
    }

    /// <summary>
    /// Processes UserDefinedTable parameters from stored procedures and creates corresponding DataContracts
    /// Returns a dictionary mapping UDT external references to their DataContract IDs for deduplication
    /// </summary>
    private Dictionary<string, string> ProcessUserDefinedTableDataContracts(
        DatabaseSchema databaseSchema,
        PackageModelPersistable package,
        MergeResult result,
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
            var dataContractName = ModelNamingUtilities.GetDataContractName(udtSchema.Name);

            var existingDataContract = IntentModelMapper.FindElementWithPrecedence(
                package.Classes,
                udtExternalRef,
                dataContractName,
                udtSchema.Schema,
                Constants.SpecializationTypes.DataContract.SpecializationType,
                package);

            ElementPersistable dataContract;
            if (existingDataContract != null)
            {
                // Update existing DataContract
                var schemaFolder = GetOrCreateSchemaFolder(udtSchema.Schema, package);
                dataContract = IntentModelMapper.CreateDataContractForUserDefinedTable(udtSchema, schemaFolder.Id, _config, package, deduplicationContext);
                SyncElements(package, existingDataContract, dataContract, _config.AllowDeletions, _config.PreserveAttributeTypes, result);
                dataContract = existingDataContract; // Use existing for mapping
            }
            else
            {
                // Create new DataContract
                var schemaFolder = GetOrCreateSchemaFolder(udtSchema.Schema, package);
                dataContract = IntentModelMapper.CreateDataContractForUserDefinedTable(udtSchema, schemaFolder.Id, _config, package, deduplicationContext);
                package.Classes.Add(dataContract);
            }
            
            // Apply UDT stereotypes after sync
            ApplyUserDefinedTableStereotypes(udtSchema, dataContract);

            udtDataContracts[udtExternalRef] = dataContract.Id;
        }

        return udtDataContracts;
    }

    /// <summary>
    /// Re-evaluates stereotypes on an existing table class element after synchronization.
    /// This ensures that renamed attributes get proper Column stereotypes applied based on their current state.
    /// </summary>
    /// <param name="table">The table schema from the database</param>
    /// <param name="existingClass">The existing class element to re-evaluate</param>
    /// <param name="config">Import configuration</param>
    private static void ApplyTableStereotypes(TableSchema table, ElementPersistable existingClass, ImportConfiguration config)
    {
        // Re-apply table-level stereotypes
        RdbmsSchemaAnnotator.ApplyTableDetails(config, table, existingClass);

        // Re-apply column-level stereotypes for each attribute
        foreach (var column in table.Columns)
        {
            // Find the existing attribute by external reference
            var columnExternalRef = ModelNamingUtilities.GetColumnExternalReference(table.Schema, table.Name, column.Name);
            var existingAttribute = existingClass.ChildElements?.FirstOrDefault(attr =>
                attr.ExternalReference == columnExternalRef && 
                attr.SpecializationType == AttributeModel.SpecializationType);

            if (existingAttribute != null)
            {
                // Re-apply all column stereotypes based on current state of the existing attribute
                RdbmsSchemaAnnotator.ApplyPrimaryKey(column, existingAttribute);
                RdbmsSchemaAnnotator.ApplyColumnDetails(column, existingAttribute);
                RdbmsSchemaAnnotator.ApplyTextConstraint(column, existingAttribute);
                RdbmsSchemaAnnotator.ApplyDecimalConstraint(column, existingAttribute);
                RdbmsSchemaAnnotator.ApplyDefaultConstraint(column, existingAttribute);
                RdbmsSchemaAnnotator.ApplyComputedValue(column, existingAttribute);
            }
        }
    }

    private static void ApplyViewStereotypes(ViewSchema view, ElementPersistable existingClass)
    {
        // Apply view-level stereotypes
        RdbmsSchemaAnnotator.ApplyViewDetails(view, existingClass);

        // Apply column-level stereotypes for views (views don't have primary keys, defaults, or computed values)
        foreach (var column in view.Columns)
        {
            // Find the corresponding attribute in the class
            var columnExternalRef = ModelNamingUtilities.GetColumnExternalReference(view.Schema, view.Name, column.Name);
            var existingAttribute = existingClass.ChildElements?.FirstOrDefault(attr =>
                attr.ExternalReference == columnExternalRef && 
                attr.SpecializationType == AttributeModel.SpecializationType);

            if (existingAttribute != null)
            {
                // Apply column stereotypes for views (limited set compared to tables)
                RdbmsSchemaAnnotator.ApplyColumnDetails(column, existingAttribute);
                RdbmsSchemaAnnotator.ApplyTextConstraint(column, existingAttribute);
                RdbmsSchemaAnnotator.ApplyDecimalConstraint(column, existingAttribute);
            }
        }
    }

    /// <summary>
    /// Applies stereotypes to stored procedure data contract elements after sync
    /// </summary>
    private static void ApplyDataContractStereotypes(StoredProcedureSchema storedProc, ElementPersistable dataContract)
    {
        // Apply stereotypes to result set columns
        foreach (var resultColumn in storedProc.ResultSetColumns)
        {
            var columnExternalRef = ModelNamingUtilities.GetResultSetColumnExternalReference(storedProc.Schema, storedProc.Name, resultColumn.Name);
            var existingAttribute = dataContract.ChildElements?.FirstOrDefault(attr =>
                attr.ExternalReference == columnExternalRef && 
                attr.SpecializationType == AttributeModel.SpecializationType);

            if (existingAttribute != null)
            {
                // Convert result set column to column schema for stereotype application
                var columnSchema = ConvertToColumnSchema(resultColumn);
                RdbmsSchemaAnnotator.ApplyColumnDetails(columnSchema, existingAttribute);
                RdbmsSchemaAnnotator.ApplyTextConstraint(columnSchema, existingAttribute);
                RdbmsSchemaAnnotator.ApplyDecimalConstraint(columnSchema, existingAttribute);
            }
        }
    }

    /// <summary>
    /// Applies stereotypes to UserDefinedTable data contract elements after sync
    /// </summary>
    private static void ApplyUserDefinedTableStereotypes(UserDefinedTableTypeSchema udtSchema, ElementPersistable dataContract)
    {
        // Apply stereotypes to UDT columns
        foreach (var column in udtSchema.Columns)
        {
            var columnExternalRef = ModelNamingUtilities.GetUserDefinedTableColumnExternalReference(udtSchema.Schema, udtSchema.Name, column.Name);
            var existingAttribute = dataContract.ChildElements?.FirstOrDefault(attr =>
                attr.ExternalReference == columnExternalRef && 
                attr.SpecializationType == AttributeModel.SpecializationType);

            if (existingAttribute != null)
            {
                // Apply column stereotypes to UDT columns
                RdbmsSchemaAnnotator.ApplyColumnDetails(column, existingAttribute);
                RdbmsSchemaAnnotator.ApplyTextConstraint(column, existingAttribute);
                RdbmsSchemaAnnotator.ApplyDecimalConstraint(column, existingAttribute);
            }
        }
    }

    /// <summary>
    /// Applies stereotypes to schema folder elements after sync
    /// </summary>
    private static void ApplySchemaFolderStereotypes(string schemaName, ElementPersistable folder)
    {
        RdbmsSchemaAnnotator.AddSchemaStereotype(folder, schemaName);
    }

    /// <summary>
    /// Converts ResultSetColumnSchema to ColumnSchema for stereotype application
    /// This allows reuse of existing stereotype logic
    /// </summary>
    private static ColumnSchema ConvertToColumnSchema(ResultSetColumnSchema resultColumn)
    {
        return new ColumnSchema
        {
            Name = resultColumn.Name,
            LanguageDataType = resultColumn.LanguageDataType,
            DbDataType = resultColumn.DbDataType,
            IsNullable = resultColumn.IsNullable,
            MaxLength = resultColumn.MaxLength,
            NumericPrecision = resultColumn.NumericPrecision,
            NumericScale = resultColumn.NumericScale,
            IsPrimaryKey = false, // Result set columns are never primary keys
            IsIdentity = false,   // Result set columns are never identity
            DefaultConstraint = null, // Result set columns don't have defaults
            ComputedColumn = null     // Result set columns aren't computed
        };
    }
}

