using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.Templates;
using Intent.RelationalDbSchemaImporter.Contracts.DbSchema;
using Intent.RelationalDbSchemaImporter.Contracts.Enums;

namespace Intent.Modules.Rdbms.Importer.Tasks.Mappers;

/// <summary>
/// Maps the <see cref="Intent.RelationalDbSchemaImporter.Contracts.DbSchema.DatabaseSchema"/> types to 
/// Intent Architect Persistence Model types.
/// </summary>
internal static class IntentModelMapper
{
    /// <summary>
    /// Finds an element using 3-level precedence:
    /// 1. ExternalReference + SpecializationType
    /// 2. Name + DB Schema + SpecializationType (using stereotype extraction)
    /// 3. Name + SpecializationType
    /// </summary>
    /// <param name="elements">Collection of elements to search</param>
    /// <param name="externalReference">External reference to match (level 1)</param>
    /// <param name="name">Element name to match (levels 2 and 3)</param>
    /// <param name="dbSchema">Database schema name to match (level 2)</param>
    /// <param name="specializationType">Specialization type to match (all levels)</param>
    /// <param name="package">Package containing the elements (for schema lookup in hierarchy)</param>
    /// <returns>Found element or null</returns>
    public static ElementPersistable? FindElementWithPrecedence(
        IEnumerable<ElementPersistable> elements,
        string? externalReference,
        string? name,
        string? dbSchema,
        string specializationType,
        PackageModelPersistable? package = null)
    {
        var elementList = elements.ToList();

        // Level 1: ExternalReference + SpecializationType
        if (!string.IsNullOrWhiteSpace(externalReference))
        {
            var byExternalRef = elementList.FirstOrDefault(e => 
                e.ExternalReference == externalReference && 
                e.SpecializationType == specializationType);
            if (byExternalRef != null)
                return byExternalRef;
        }

        // Level 2: Name + DB Schema + SpecializationType (using stereotype extraction)
        if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(dbSchema))
        {
            var byNameAndSchema = elementList.FirstOrDefault(e => 
                e.Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
                e.SpecializationType == specializationType &&
                GetElementDbSchema(e, package) == dbSchema);
            if (byNameAndSchema != null)
                return byNameAndSchema;
        }

        // Level 3: Name + SpecializationType (but avoid matching if schemas conflict)
        if (!string.IsNullOrWhiteSpace(name))
        {
            var byName = elementList.FirstOrDefault(e => 
                e.Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
                e.SpecializationType == specializationType &&
                // Only match if schemas are compatible (both null/empty or both equal)
                (string.IsNullOrWhiteSpace(dbSchema) || 
                 string.IsNullOrWhiteSpace(GetElementDbSchema(e, package)) || 
                 GetElementDbSchema(e, package) == dbSchema));
            if (byName != null)
                return byName;
        }

        return null;
    }

    /// <summary>
    /// Extracts the database schema from an element using stereotype properties.
    /// Checks the element itself, then parent folder, then package hierarchy.
    /// </summary>
    internal static string? GetElementDbSchema(ElementPersistable element, PackageModelPersistable? package = null)
    {
        // Try Table stereotype on element first
        if (element.TryGetStereotypeProperty(
            Constants.Stereotypes.Rdbms.Table.DefinitionId,
            Constants.Stereotypes.Rdbms.Table.PropertyId.Schema,
            out var tableSchema))
        {
            return tableSchema;
        }

        // Try View stereotype on element
        if (element.TryGetStereotypeProperty(
            Constants.Stereotypes.Rdbms.View.DefinitionId,
            Constants.Stereotypes.Rdbms.View.PropertyId.Schema,
            out var viewSchema))
        {
            return viewSchema;
        }

        // Try Schema stereotype on element (for folders)
        if (element.TryGetStereotypeProperty(
            Constants.Stereotypes.Rdbms.Schema.DefinitionId,
            Constants.Stereotypes.Rdbms.Schema.PropertyId.Name,
            out var schemaName))
        {
            return schemaName;
        }

        // If package is provided, check parent folder hierarchy
        if (package != null && !string.IsNullOrWhiteSpace(element.ParentFolderId))
        {
            var folder = FindFolderById(package, element.ParentFolderId);
            if (folder != null)
            {
                // Check Schema stereotype on folder
                if (folder.TryGetStereotypeProperty(
                    Constants.Stereotypes.Rdbms.Schema.DefinitionId,
                    Constants.Stereotypes.Rdbms.Schema.PropertyId.Name,
                    out var folderSchema))
                {
                    return folderSchema;
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Finds a folder by ID in the package (folders are stored as elements in Classes collection)
    /// </summary>
    private static ElementPersistable? FindFolderById(PackageModelPersistable package, string folderId)
    {
        // Folders are stored in the Classes collection with Folder specialization type
        return package.Classes.FirstOrDefault(e => 
            e.Id == folderId && 
            e.SpecializationType == Constants.SpecializationTypes.Folder.SpecializationType);
    }

    public static ElementPersistable MapTableToClass(
        TableSchema table, 
        ImportConfiguration config, 
        PackageModelPersistable package,
        string? parentFolderId = null,
        DeduplicationContext? deduplicationContext = null)
    {
        var className = ModelNamingUtilities.GetEntityName(table.Name, config.EntityNameConvention, table.Schema, deduplicationContext, ['_']);
        var classElement = new ElementPersistable
        {
            Id = Guid.NewGuid().ToString(),
            SpecializationType = ClassModel.SpecializationType,
            SpecializationTypeId = ClassModel.SpecializationTypeId,
            Name = className,
            Display = className,
            ExternalReference = ModelNamingUtilities.GetTableExternalReference(table.Schema, table.Name),
            IsAbstract = false,
            SortChildren = SortChildrenOptions.SortByTypeAndName,
            GenericTypes = [],
            IsMapped = false,
            ParentFolderId = parentFolderId!, // Set parent folder for schema organization
            PackageId = package.Id,
            PackageName = package.Name,
            Stereotypes = [],
            Metadata = [],
            ChildElements = []
        };

        // Map columns to attributes
        foreach (var column in table.Columns)
        {
            var attribute = MapColumnToAttribute(column, table.Name, className, table.Schema, config, package, classElement.Id, deduplicationContext);
            classElement.ChildElements.Add(attribute);

            // Stereotypes will be applied by DbSchemaIntentMetadataMerger after sync
        }

        // Table stereotypes will be applied by DbSchemaIntentMetadataMerger after sync

        // Map triggers to child elements
        foreach (var trigger in table.Triggers)
        {
            var triggerExternalRef = ModelNamingUtilities.GetTriggerExternalReference(table.Schema, table.Name, trigger.Name);
            var triggerElement = package.Classes
                .FirstOrDefault(x => x.ExternalReference == triggerExternalRef && 
                                     x.SpecializationType == Constants.SpecializationTypes.Trigger.SpecializationType);
            if (triggerElement is null)
            {
                triggerElement = MapTriggerToElement(trigger, table.Name, table.Schema, classElement.Id, package);
                classElement.ChildElements.Add(triggerElement);
            }
        }

        return classElement;
    }

    public static ElementPersistable MapViewToClass(
        ViewSchema view, 
        ImportConfiguration config, 
        PackageModelPersistable package,
        string? parentFolderId = null, 
        DeduplicationContext? deduplicationContext = null)
    {
        var className = ModelNamingUtilities.GetViewName(view.Name, config.EntityNameConvention, view.Schema, deduplicationContext);
        var classElement = new ElementPersistable
        {
            Id = Guid.NewGuid().ToString(),
            SpecializationType = ClassModel.SpecializationType,
            SpecializationTypeId = ClassModel.SpecializationTypeId,
            Name = className,
            Display = className,
            ExternalReference = ModelNamingUtilities.GetViewExternalReference(view.Schema, view.Name),
            IsAbstract = false,
            SortChildren = SortChildrenOptions.SortByTypeAndName,
            GenericTypes = [],
            IsMapped = false,
            ParentFolderId = parentFolderId!, // Set parent folder for schema organization
            PackageId = package.Id,
            PackageName = package.Name,
            Stereotypes = [],
            Metadata = [],
            ChildElements = []
        };

        // Map columns to attributes
        foreach (var column in view.Columns)
        {
            var attribute = MapColumnToAttribute(column, view.Name, className, view.Schema, config, package, classElement.Id, deduplicationContext);
            classElement.ChildElements.Add(attribute);

            // Stereotypes will be applied by DbSchemaIntentMetadataMerger after sync
        }

        // View stereotypes will be applied by DbSchemaIntentMetadataMerger after sync

        return classElement;
    }
    
    /// <summary>
    /// Creates a repository element for containing stored procedures/operations
    /// </summary>
    public static ElementPersistable CreateRepository(string repositoryName, string schemaFolderId, PackageModelPersistable package, string externalReference)
    {
        return new ElementPersistable
        {
            Id = Guid.NewGuid().ToString(),
            SpecializationType = Constants.SpecializationTypes.Repository.SpecializationType,
            SpecializationTypeId = Constants.SpecializationTypes.Repository.SpecializationTypeId,
            ExternalReference = externalReference,
            Name = repositoryName,
            Display = repositoryName,
            IsAbstract = false,
            SortChildren = SortChildrenOptions.Manually,
            GenericTypes = [],
            TypeReference = new TypeReferencePersistable
            {
                Id = Guid.NewGuid().ToString(),
                IsNavigable = true,
                IsNullable = false,
                IsCollection = false,
                IsRequired = true,
                Stereotypes = [],
                GenericTypeParameters = []
            },
            IsMapped = false,
            ParentFolderId = schemaFolderId, // Repositories belong to schema folders
            PackageId = package.Id,
            PackageName = package.Name,
            Stereotypes = [],
            Metadata = [],
            ChildElements = []
        };
    }

    /// <summary>
    /// Maps stored procedure to an element in Repository with UserDefinedTable DataContract support
    /// </summary>
    public static ElementPersistable MapStoredProcedureToElement(
        StoredProcedureSchema storedProc, 
        string? repositoryId, 
        PackageModelPersistable package, 
        DeduplicationContext? deduplicationContext,
        Dictionary<string, string>? udtDataContracts)
    {
        var procName = ModelNamingUtilities.GetStoredProcedureName(storedProc.Name, storedProc.Schema, deduplicationContext);

        var procElement = new ElementPersistable
        {
            Id = Guid.NewGuid().ToString(),
            SpecializationType = Constants.SpecializationTypes.StoredProcedure.SpecializationType,
            SpecializationTypeId = Constants.SpecializationTypes.StoredProcedure.SpecializationTypeId,
            Name = procName,
            Display = procName,
            ExternalReference = ModelNamingUtilities.GetStoredProcedureExternalReference(storedProc.Schema, storedProc.Name),
            IsAbstract = false,
            GenericTypes = [],
            TypeReference = new TypeReferencePersistable
            {
                Id = Guid.NewGuid().ToString(),
                IsNavigable = true,
                IsNullable = false,
                IsCollection = false,
                IsRequired = true,
                Stereotypes = [],
                GenericTypeParameters = []
            },
            IsMapped = false,
            ParentFolderId = repositoryId!, // Stored procedures belong to the repository
            PackageId = package.Id,
            PackageName = package.Name,
            Stereotypes = [],
            Metadata = [],
            ChildElements = []
        };

        // Map parameters
        foreach (var parameter in storedProc.Parameters)
        {
            var paramElement = MapStoredProcParameterToElement(parameter, procElement.Id, package, udtDataContracts);
            procElement.ChildElements.Add(paramElement);
        }
        
        return procElement;
    }

    /// <summary>
    /// Maps a stored procedure to an operation (Operation mode) with UserDefinedTable DataContract support
    /// </summary>
    public static ElementPersistable MapStoredProcedureToOperation(
        StoredProcedureSchema storedProc, 
        string repositoryId, 
        PackageModelPersistable package, 
        DeduplicationContext? deduplicationContext,
        Dictionary<string, string>? udtDataContracts)
    {
        var procName = ModelNamingUtilities.GetStoredProcedureName(storedProc.Name, storedProc.Schema, deduplicationContext);

        var operationElement = new ElementPersistable
        {
            Id = Guid.NewGuid().ToString(),
            SpecializationType = Constants.SpecializationTypes.Operation.SpecializationType,
            SpecializationTypeId = Constants.SpecializationTypes.Operation.SpecializationTypeId,
            Name = procName,
            Display = procName,
            ExternalReference = ModelNamingUtilities.GetStoredProcedureExternalReference(storedProc.Schema, storedProc.Name),
            IsAbstract = false,
            SortChildren = SortChildrenOptions.SortByTypeThenManually,
            GenericTypes = [],
            TypeReference = new TypeReferencePersistable
            {
                Id = Guid.NewGuid().ToString(),
                IsNavigable = true,
                IsNullable = false,
                IsCollection = false,
                IsRequired = true,
                Stereotypes = [],
                GenericTypeParameters = []
            },
            IsMapped = false,
            ParentFolderId = repositoryId, // Operations belong to repository
            PackageId = package.Id,
            PackageName = package.Name,
            Traits = 
            [
                new ImplementedTraitPersistable
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "[Invokable]"
                }
            ],
            Stereotypes = [],
            Metadata = [],
            ChildElements = []
        };

        // Map parameters
        foreach (var parameter in storedProc.Parameters)
        {
            var paramElement = MapStoredProcParameterToOperation(parameter, operationElement.Id, package, udtDataContracts);
            operationElement.ChildElements.Add(paramElement);
        }

        return operationElement;
    }

    /// <summary>
    /// Creates a data contract for stored procedure result set
    /// Following the pattern from old DatabaseSchemaToModelMapper.GetOrCreateDataContractResponse
    /// </summary>
    public static ElementPersistable CreateDataContractForStoredProcedure(StoredProcedureSchema storedProc, string schemaFolderId, string storedProcElementName, ImportConfiguration config, PackageModelPersistable package, DeduplicationContext? deduplicationContext)
    {
        var dataContractName = $"{storedProcElementName}Response";
        
        var dataContract = new ElementPersistable
        {
            Id = Guid.NewGuid().ToString(),
            SpecializationType = Constants.SpecializationTypes.DataContract.SpecializationType,
            SpecializationTypeId = Constants.SpecializationTypes.DataContract.SpecializationTypeId,
            Name = dataContractName,
            Display = dataContractName,
            ExternalReference = ModelNamingUtilities.GetDataContractExternalReference(storedProc.Schema, storedProc.Name),
            IsAbstract = false,
            SortChildren = SortChildrenOptions.SortByTypeThenManually,
            GenericTypes = [],
            IsMapped = false,
            ParentFolderId = schemaFolderId, // Data contracts belong to schema folder (same as stored proc)
            PackageId = package.Id,
            PackageName = package.Name,
            Stereotypes = [],
            Metadata = [],
            ChildElements = []
        };

        // Handle result set columns
        foreach (var resultColumn in storedProc.ResultSetColumns)
        {
            var attribute = MapResultSetColumnToAttribute(resultColumn, dataContract.Id, storedProc.Name, storedProc.Schema, resultColumn.Name, config, deduplicationContext);
            dataContract.ChildElements.Add(attribute);

            // Stereotypes will be applied by DbSchemaIntentMetadataMerger after sync
        }

        return dataContract;
    }

    /// <summary>
    /// Creates a data contract for UserDefinedTable type
    /// Follows a similar pattern to CreateDataContractForStoredProcedure but for UDT columns
    /// </summary>
    public static ElementPersistable CreateDataContractForUserDefinedTable(
        UserDefinedTableTypeSchema udtSchema, 
        string schemaFolderId, 
        ImportConfiguration config,
        PackageModelPersistable package, 
        DeduplicationContext? deduplicationContext)
    {
        var dataContractName = ModelNamingUtilities.GetDataContractName(udtSchema.Name);
        
        var dataContract = new ElementPersistable
        {
            Id = Guid.NewGuid().ToString(),
            SpecializationType = Constants.SpecializationTypes.DataContract.SpecializationType,
            SpecializationTypeId = Constants.SpecializationTypes.DataContract.SpecializationTypeId,
            Name = dataContractName,
            Display = dataContractName,
            ExternalReference = ModelNamingUtilities.GetUserDefinedTableDataContractExternalReference(udtSchema.Schema, udtSchema.Name),
            IsAbstract = false,
            SortChildren = SortChildrenOptions.SortByTypeThenManually,
            GenericTypes = [],
            IsMapped = false,
            ParentFolderId = schemaFolderId, // Data contracts belong to schema folder
            PackageId = package.Id,
            PackageName = package.Name,
            Stereotypes = [],
            Metadata = [],
            ChildElements = []
        };

        // Map UDT columns to attributes
        foreach (var column in udtSchema.Columns)
        {
            var attribute = MapUserDefinedTableColumnToAttribute(column, dataContract.Id, udtSchema.Name, udtSchema.Schema, column.Name, config, deduplicationContext);
            dataContract.ChildElements.Add(attribute);

            // Stereotypes will be applied by DbSchemaIntentMetadataMerger after sync
        }

        return dataContract;
    }
    
    /// <summary>
    /// Creates an index element with proper mapping to attributes
    /// </summary>
    public static ElementPersistable CreateIndex(TableSchema table, IndexSchema index, string classId, PackageModelPersistable package)
    {
        ArgumentException.ThrowIfNullOrEmpty(classId);
        var indexElement = new ElementPersistable
        {
            Id = Guid.NewGuid().ToString(),
            SpecializationType = Constants.SpecializationTypes.Index.SpecializationType,
            SpecializationTypeId = Constants.SpecializationTypes.Index.SpecializationTypeId,
            Name = index.Name,
            Display = index.Name,
            ExternalReference = ModelNamingUtilities.GetIndexExternalReference(table.Schema, table.Name, index.Name),
            IsAbstract = false,
            SortChildren = SortChildrenOptions.Manually,
            GenericTypes = [],
            IsMapped = true,
            Mapping = new BasicMappingModelPersistable
            {
                IsRootMapping = true,
                ApplicationId = package.ApplicationId,
                MetadataId = Constants.Mapping.Index.MetadataId, // Domain metadata ID
                MappingSettingsId = Constants.Mapping.Index.MappingSettingsId, // Column mapping settings ID
                AutoSyncTypeReference = false,
                Path = new List<MappedPathTargetPersistable>
                {
                    new()
                    {
                        Id = classId,
                        Name = GetClassNameById(classId, package),
                        Type = "Element",
                        Specialization = ClassModel.SpecializationType,
                        SpecializationId = ClassModel.SpecializationTypeId
                    }
                }
            },
            ParentFolderId = classId, // Indexes belong to their parent class
            PackageId = package.Id,
            PackageName = package.Name,
            Stereotypes = [],
            Metadata = [],
            ChildElements = []
        };

        // Index stereotypes will be applied by DbSchemaIntentMetadataMerger after sync

        return indexElement;
    }

    /// <summary>
    /// Creates an index column element with mapping to the corresponding attribute
    /// </summary>
    public static ElementPersistable CreateIndexColumn(IndexColumnSchema indexColumn, string indexId, string attributeId, PackageModelPersistable package)
    {
        ArgumentException.ThrowIfNullOrEmpty(indexId);
        ArgumentException.ThrowIfNullOrEmpty(attributeId);
        
        var mapping = new BasicMappingModelPersistable
        {
            MappingSettingsId = Constants.Mapping.Index.MappingSettingsId, // Column mapping settings ID
            MetadataId = Constants.Mapping.Index.MetadataId, // Domain metadata ID
            AutoSyncTypeReference = false,
            Path = new List<MappedPathTargetPersistable>
            {
                new()
                {
                    Id = attributeId,
                    Name = GetAttributeNameById(attributeId, package),
                    Type = "Element",
                    Specialization = "Attribute"
                }
            }
        };

        var columnIndex = new ElementPersistable
        {
            Id = Guid.NewGuid().ToString(),
            SpecializationType = Constants.SpecializationTypes.IndexColumn.SpecializationType,
            SpecializationTypeId = Constants.SpecializationTypes.IndexColumn.SpecializationTypeId,
            Name = indexColumn.Name,
            Display = indexColumn.Name,
            ExternalReference = ModelNamingUtilities.GetIndexColumnExternalReference(indexColumn.Name),
            IsAbstract = false,
            GenericTypes = [],
            IsMapped = true,
            Mapping = mapping,
            ParentFolderId = indexId, // Index columns belong to their parent index
            PackageId = package.Id,
            PackageName = package.Name,
            Stereotypes = [],
            Metadata = [],
            ChildElements = []
        };

        // Index column stereotypes will be applied by DbSchemaIntentMetadataMerger after sync

        return columnIndex;
    }
    
    /// <summary>
    /// Gets or creates an association based on foreign key information
    /// Ported from original DatabaseSchemaToModelMapper.GetOrCreateAssociation
    /// </summary>
    /// <param name="foreignKey">The foreign key to create an association for</param>
    /// <param name="sourceTable">The source table schema</param>
    /// <param name="sourceClass">The source class element</param>
    /// <param name="package">The package to add the association to</param>
    /// <param name="databaseSchema">The complete database schema for validation</param>
    /// <returns>Status indicating the outcome of the association creation attempt</returns>
    public static AssociationCreationResult GetOrCreateAssociation(
        ForeignKeySchema foreignKey, 
        TableSchema sourceTable, 
        ElementPersistable sourceClass, 
        PackageModelPersistable package,
        DatabaseSchema databaseSchema)
    {
        var targetTableExternalRef = ModelNamingUtilities.GetTableExternalReference(foreignKey.ReferencedTableSchema, foreignKey.ReferencedTableName);
        var targetTableEntity = ModelNamingUtilities.GetEntityName(foreignKey.ReferencedTableName, EntityNameConvention.SingularEntity, foreignKey.ReferencedTableSchema, null, []);
        
        // Use unified lookup helper with 3-level precedence
        var targetClass = FindElementWithPrecedence(
            package.Classes,
            targetTableExternalRef,
            targetTableEntity,
            foreignKey.ReferencedTableSchema,
            ClassModel.SpecializationType,
            package);

        if (targetClass == null)
        {
            return AssociationCreationResult.TargetClassNotFound(foreignKey);
        }

        // Validate foreign key structure for association support
        var validationResult = ValidateForeignKeyForAssociation(foreignKey, sourceTable, databaseSchema);
        if (validationResult != null)
        {
            return validationResult;
        }

        // Generate a target name based on foreign key column naming
        string targetName;
        var singularTableName = foreignKey.ReferencedTableName.Singularize();
        var firstColumnName = foreignKey.Columns.First().Name;

        // NOTE: If you make any changes in here, make sure the RDBMS FK naming convention is also up to date!
        
        // Determine association target name based on column naming patterns
        // For multiple FKs to the same table, use the column name + target table name for uniqueness
        if (firstColumnName.Equals($"{singularTableName}Id", StringComparison.OrdinalIgnoreCase))
        {
            // Standard pattern: FKTableId -> FKTable
            targetName = singularTableName;
        }
        else if (firstColumnName.EndsWith("Id", StringComparison.OrdinalIgnoreCase))
        {
            // Custom naming pattern: FKTableId1, FKWithTableId2, etc. -> FKTableId1FKTable, FKWithTableId2FKTable
            var columnNameWithoutId = firstColumnName.Substring(0, firstColumnName.Length - 2);
            targetName = $"{columnNameWithoutId}{singularTableName}";
        }
        else
        {
            // Fallback: use column name + table name
            targetName = $"{firstColumnName}{singularTableName}";
        }

        // Generate external reference for a foreign key
        var fkExternalRef = ModelNamingUtilities.GetForeignKeyExternalReference(sourceTable.Schema, sourceTable.Name, foreignKey.Name);

        // Check if the association already exists by foreign key external reference
        var existingAssociation = package.Associations?.FirstOrDefault(a => a.ExternalReference == fkExternalRef);

        if (existingAssociation == null)
        {
            // Determine if this is a one-to-one relationship (FK columns are all primary keys)
            var sourcePkColumns = sourceTable.Columns.Where(c => c.IsPrimaryKey).Select(c => c.Name).ToHashSet();
            var fkColumnNames = foreignKey.Columns.Select(c => c.Name).ToHashSet();
            var isOneToOne = sourcePkColumns.Count == fkColumnNames.Count && 
                           fkColumnNames.All(fk => sourcePkColumns.Contains(fk));

            // Check if any FK columns are nullable to determine association nullability
            var isNullable = foreignKey.Columns.Any(fkCol => 
                sourceTable.Columns.Any(col => col.Name == fkCol.Name && col.IsNullable));

            // Avoid naming conflicts with source table name
            var finalTargetName = targetName.Equals(sourceTable.Name.Singularize(), StringComparison.OrdinalIgnoreCase) 
                ? $"{targetName}Reference" 
                : targetName;

            var finalSourceName = isOneToOne ? sourceTable.Name.Singularize() : sourceTable.Name.Pluralize();

            var associationId = Guid.NewGuid().ToString();
            var newAssociation = new AssociationPersistable
            {
                Id = associationId,
                ExternalReference = fkExternalRef,
                AssociationTypeId = Constants.SpecializationTypes.Association.SpecializationTypeId,
                AssociationType = Constants.SpecializationTypes.Association.SpecializationType,
                TargetEnd = new AssociationEndPersistable
                {
                    Id = associationId, // Keep same as association ID for target end
                    Name = finalTargetName,
                    SpecializationType = AssociationTargetEndModel.SpecializationType,
                    SpecializationTypeId = AssociationTargetEndModel.SpecializationTypeId,
                    TypeReference = new TypeReferencePersistable
                    {
                        Id = Guid.NewGuid().ToString(),
                        TypeId = targetClass.Id,
                        IsNavigable = true,
                        IsNullable = isNullable,
                        IsCollection = false
                    },
                    Stereotypes = [],
                    ExternalReference = fkExternalRef // Link target end to the foreign key
                },
                SourceEnd = new AssociationEndPersistable
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = finalSourceName,
                    SpecializationType = AssociationSourceEndModel.SpecializationType,
                    SpecializationTypeId = AssociationSourceEndModel.SpecializationTypeId,
                    TypeReference = new TypeReferencePersistable
                    {
                        Id = Guid.NewGuid().ToString(),
                        TypeId = sourceClass.Id,
                        IsNavigable = false,
                        IsNullable = false,
                        IsCollection = !isOneToOne // One-to-many unless it's one-to-one
                    },
                    Stereotypes = []
                }
            };

            // Check for reverse ownership (manually modeled associations)
            if (!SameAssociationExistsWithReverseOwnership(package.Associations?.ToList(), newAssociation))
            {
                package.Associations ??= new List<AssociationPersistable>();
                package.Associations.Add(newAssociation);
            }
            
            return AssociationCreationResult.Success(newAssociation);
        }
        
        return AssociationCreationResult.Success(existingAssociation);
    }
    
    /// <summary>
    /// Creates a folder element for schema organization
    /// </summary>
    public static ElementPersistable CreateSchemaFolder(string schemaName, PackageModelPersistable package)
    {
        var folderSchemaName = ModelNamingUtilities.GetFolderName(schemaName);
        var folder = new ElementPersistable
        {
            Id = Guid.NewGuid().ToString(),
            SpecializationType = Constants.SpecializationTypes.Folder.SpecializationType,
            SpecializationTypeId = Constants.SpecializationTypes.Folder.SpecializationTypeId,
            Name = folderSchemaName,
            Display = folderSchemaName,
            ExternalReference = ModelNamingUtilities.GetSchemaExternalReference(schemaName),
            IsAbstract = false,
            SortChildren = SortChildrenOptions.SortByTypeAndName,
            GenericTypes = [],
            IsMapped = false,
            ParentFolderId = package.Id,
            PackageId = package.Id,
            PackageName = package.Name,
            Stereotypes = [],
            Metadata = [],
            ChildElements = []
        };

        // Schema stereotypes will be applied by DbSchemaIntentMetadataMerger after sync
        
        return folder;
    }

    private static ElementPersistable MapColumnToAttribute(
        ColumnSchema column, 
        string tableName, 
        string className, 
        string schema, 
        ImportConfiguration config,
        PackageModelPersistable package,
        string? parentClassId = null, 
        DeduplicationContext? deduplicationContext = null)
    {
        var attributeName = ModelNamingUtilities.GetAttributeName(column.Name, tableName, className, schema, config.AttributeNameConvention, deduplicationContext);
        
        return new ElementPersistable
        {
            Id = Guid.NewGuid().ToString(),
            SpecializationType = AttributeModel.SpecializationType,
            SpecializationTypeId = AttributeModel.SpecializationTypeId,
            Name = attributeName,
            Display = attributeName,
            ExternalReference = ModelNamingUtilities.GetColumnExternalReference(schema, tableName, column.Name),
            IsAbstract = false,
            GenericTypes = [],
            TypeReference = TypeReferenceMapper.MapColumnTypeToTypeReference(column),
            IsMapped = false,
            ParentFolderId = parentClassId!, // Attributes belong to their parent class
            PackageId = package.Id,
            PackageName = package.Name,
            Stereotypes = [],
            Metadata = [],
            ChildElements = []
        };
    }

    /// <summary>
    /// Gets the appropriate TypeReference for a stored procedure parameter, handling UserDefinedTable DataContracts
    /// </summary>
    private static TypeReferencePersistable GetParameterTypeReference(StoredProcedureParameterSchema parameter, Dictionary<string, string>? udtDataContracts)
    {
        // Check if this parameter uses a UserDefinedTable and we have a DataContract for it
        if (parameter.UserDefinedTableType != null && udtDataContracts != null)
        {
            var udtExternalRef = ModelNamingUtilities.GetUserDefinedTableDataContractExternalReference(
                parameter.UserDefinedTableType.Schema,
                parameter.UserDefinedTableType.Name);
            
            if (udtDataContracts.TryGetValue(udtExternalRef, out var dataContractId))
            {
                return TypeReferenceMapper.MapStoredProcedureParameterTypeToTypeReference(parameter, dataContractId);
            }
        }

        // Fall back to standard mapping
        return TypeReferenceMapper.MapStoredProcedureParameterTypeToTypeReference(parameter);
    }

    /// <summary>
    /// Maps a stored procedure parameter to a stored procedure element with UserDefinedTable DataContract support
    /// </summary>
    private static ElementPersistable MapStoredProcParameterToElement(StoredProcedureParameterSchema parameter, string storedProcId, PackageModelPersistable package, Dictionary<string, string>? udtDataContracts)
    {
        var paramName = ModelNamingUtilities.GetParameterName(parameter.Name);

        return new ElementPersistable
        {
            Id = Guid.NewGuid().ToString(),
            SpecializationType = Constants.SpecializationTypes.StoredProcedureParameter.SpecializationType,
            SpecializationTypeId = Constants.SpecializationTypes.StoredProcedureParameter.SpecializationTypeId,
            Name = paramName,
            Display = paramName,
            ExternalReference = paramName.ToLowerInvariant(),
            IsAbstract = false,
            GenericTypes = [],
            TypeReference = GetParameterTypeReference(parameter, udtDataContracts),
            IsMapped = false,
            ParentFolderId = storedProcId, // Parameters belong to stored procedure
            PackageId = package.Id,
            PackageName = package.Name,
            Stereotypes = [],
            Metadata = [],
            ChildElements = []
        };
    }

    /// <summary>
    /// Maps a stored procedure parameter to an operation parameter with UserDefinedTable DataContract support
    /// </summary>
    private static ElementPersistable MapStoredProcParameterToOperation(StoredProcedureParameterSchema parameter, string operationId, PackageModelPersistable package, Dictionary<string, string>? udtDataContracts)
    {
        var paramName = ModelNamingUtilities.GetParameterName(parameter.Name);

        return new ElementPersistable
        {
            Id = Guid.NewGuid().ToString(),
            SpecializationType = Constants.SpecializationTypes.Parameter.SpecializationType,
            SpecializationTypeId = Constants.SpecializationTypes.Parameter.SpecializationTypeId,
            Name = paramName,
            Display = paramName,
            ExternalReference = paramName.ToLowerInvariant(),
            IsAbstract = false,
            GenericTypes = [],
            TypeReference = GetParameterTypeReference(parameter, udtDataContracts),
            IsMapped = false,
            ParentFolderId = operationId, // Parameters belong to operation
            PackageId = package.Id,
            PackageName = package.Name,
            Stereotypes = [],
            Metadata = [],
            ChildElements = []
        };
    }

    /// <summary>
    /// Maps a result set column to an attribute element
    /// </summary>
    private static ElementPersistable MapResultSetColumnToAttribute(ResultSetColumnSchema resultColumn, string dataContractId, string procName, string schema, string resultColumnName, ImportConfiguration config, DeduplicationContext? deduplicationContext)
    {
        var attributeName = ModelNamingUtilities.GetAttributeName(resultColumnName, null, procName, schema, config.AttributeNameConvention, deduplicationContext);
        
        return new ElementPersistable
        {
            Id = Guid.NewGuid().ToString(),
            SpecializationType = AttributeModel.SpecializationType,
            SpecializationTypeId = AttributeModel.SpecializationTypeId,
            Name = attributeName,
            ExternalReference = ModelNamingUtilities.GetResultSetColumnExternalReference(schema, procName, resultColumn.Name),
            IsAbstract = false,
            GenericTypes = [],
            TypeReference = TypeReferenceMapper.MapResultSetColumnTypeToTypeReference(resultColumn),
            IsMapped = false,
            ParentFolderId = dataContractId, // Attributes belong to their parent data contract
            Stereotypes = [],
            Metadata = [],
            ChildElements = []
        };
    }

    /// <summary>
    /// Maps a UserDefinedTable ColumnSchema to an attribute element
    /// </summary>
    private static ElementPersistable MapUserDefinedTableColumnToAttribute(ColumnSchema column, string dataContractId, string udtName, string schema, string columnName, ImportConfiguration config, DeduplicationContext? deduplicationContext)
    {
        var attributeName = ModelNamingUtilities.GetAttributeName(columnName, null, udtName, schema, config.AttributeNameConvention, deduplicationContext);
        
        return new ElementPersistable
        {
            Id = Guid.NewGuid().ToString(),
            SpecializationType = AttributeModel.SpecializationType,
            SpecializationTypeId = AttributeModel.SpecializationTypeId,
            Name = attributeName,
            ExternalReference = ModelNamingUtilities.GetUserDefinedTableColumnExternalReference(schema, udtName, column.Name),
            IsAbstract = false,
            GenericTypes = [],
            TypeReference = TypeReferenceMapper.MapColumnTypeToTypeReference(column),
            IsMapped = false,
            ParentFolderId = dataContractId, // Attributes belong to their parent data contract
            Stereotypes = [],
            Metadata = [],
            ChildElements = []
        };
    }
    
    private static string GetClassNameById(string classId, PackageModelPersistable package)
    {
        var element = package.Classes.FirstOrDefault(c => c.Id == classId);
        return element?.Name ?? "Unknown";
    }

    private static string GetAttributeNameById(string attributeId, PackageModelPersistable package)
    {
        foreach (var classElement in package.Classes)
        {
            var attribute = classElement.ChildElements?.FirstOrDefault(c => c.Id == attributeId);
            if (attribute != null)
                return attribute.Name;
        }
        return "Unknown";
    }

    /// <summary>
    /// Checks if the same association exists with reverse ownership (manually modeled)
    /// </summary>
    private static bool SameAssociationExistsWithReverseOwnership(List<AssociationPersistable>? associations, AssociationPersistable newAssociation)
    {
        if (associations == null) return false;

        return associations.Any(existing =>
            existing.TargetEnd.TypeReference?.TypeId == newAssociation.SourceEnd.TypeReference?.TypeId &&
            existing.SourceEnd.TypeReference?.TypeId == newAssociation.TargetEnd.TypeReference?.TypeId &&
            existing.SourceEnd.TypeReference?.IsNavigable == true);
    }

    private static ElementPersistable MapTriggerToElement(TriggerSchema trigger, string tableName, string schema, string parentClassId, PackageModelPersistable package)
    {
        var triggerName = trigger.Name; // Names are typically unique and descriptive
        
        var triggerElement = new ElementPersistable
        {
            Id = Guid.NewGuid().ToString(),
            SpecializationType = Constants.SpecializationTypes.Trigger.SpecializationType,
            SpecializationTypeId = Constants.SpecializationTypes.Trigger.SpecializationTypeId,
            Name = triggerName,
            Display = triggerName,
            ExternalReference = ModelNamingUtilities.GetTriggerExternalReference(schema, tableName, trigger.Name),
            IsAbstract = false,
            GenericTypes = [],
            IsMapped = false,
            ParentFolderId = parentClassId,
            PackageId = package.Id,
            PackageName = package.Name,
            Stereotypes = [],
            Metadata = [],
            ChildElements = []
        };
        
        return triggerElement;
    }

    /// <summary>
    /// Validates whether a foreign key structure is supported for association creation
    /// </summary>
    /// <param name="foreignKey">The foreign key to validate</param>
    /// <param name="sourceTable">The source table containing the foreign key</param>
    /// <param name="databaseSchema">The complete database schema for validation</param>
    /// <returns>AssociationCreationResult with error details if validation fails, null if validation passes</returns>
    private static AssociationCreationResult? ValidateForeignKeyForAssociation(
        ForeignKeySchema foreignKey, 
        TableSchema sourceTable, 
        DatabaseSchema databaseSchema)
    {
        // Find the referenced table from the imported schema
        // Note: This only checks tables that are being imported in this operation
        var referencedTable = databaseSchema.Tables.FirstOrDefault(t => 
            string.Equals(t.Name, foreignKey.ReferencedTableName, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(t.Schema, foreignKey.ReferencedTableSchema, StringComparison.OrdinalIgnoreCase));

        if (referencedTable == null)
        {
            // The referenced table is not in the current import set
            // This could be because:
            // 1. It's an inclusive import and the table wasn't selected
            // 2. The table genuinely doesn't exist in the database
            // We'll return UnsupportedForeignKey with a clear message for inclusive imports
            return AssociationCreationResult.UnsupportedForeignKey(foreignKey, 
                $"Referenced table '{foreignKey.ReferencedTableSchema}.{foreignKey.ReferencedTableName}' is not included in the current import. " +
                $"This may be due to an inclusive import where only specific tables are selected, or the table could not be found in the database schema being imported.",
                sourceTable.Schema);
        }

        // Get primary key columns of the referenced table
        var referencedTablePrimaryKeys = referencedTable.Columns
            .Where(c => c.IsPrimaryKey)
            .Select(c => c.Name)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        if (referencedTablePrimaryKeys.Count == 0)
        {
            return AssociationCreationResult.UnsupportedForeignKey(foreignKey,
                $"Referenced table '{foreignKey.ReferencedTableSchema}.{foreignKey.ReferencedTableName}' has no primary key defined",
                sourceTable.Schema);
        }

        // Check if foreign key references all and only the primary key columns
        var foreignKeyReferencedColumns = foreignKey.Columns
            .Select(c => c.ReferencedColumnName)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        
        if (!foreignKeyReferencedColumns.SetEquals(referencedTablePrimaryKeys))
        {
            return AssociationCreationResult.UnsupportedForeignKey(foreignKey,
                $"Foreign key references non-primary key columns. " +
                $"FK references: [{string.Join(", ", foreignKey.Columns.Select(c => c.ReferencedColumnName))}], " +
                $"PK columns: [{string.Join(", ", referencedTablePrimaryKeys)}]",
                sourceTable.Schema);
        }

        // If we reach here, the foreign key is valid for association creation
        return null;
    }
}
