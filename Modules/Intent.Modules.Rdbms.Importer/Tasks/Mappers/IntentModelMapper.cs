using System;
using System.Collections.Generic;
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
    public static ElementPersistable MapTableToClass(
        TableSchema table, 
        ImportConfiguration config, 
        PackageModelPersistable package,
        string? parentFolderId = null,
        DeduplicationContext? deduplicationContext = null)
    {
        var className = ModelNamingUtilities.GetEntityName(table.Name, config.EntityNameConvention, table.Schema, deduplicationContext);
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
            ParentFolderId = parentFolderId, // Set parent folder for schema organization
            PackageId = package.Id,
            PackageName = package.Name,
            Stereotypes = [],
            Metadata = [],
            ChildElements = []
        };

        // Map columns to attributes
        foreach (var column in table.Columns)
        {
            var attribute = MapColumnToAttribute(column, table.Name, className, table.Schema, package, classElement.Id, deduplicationContext);
            classElement.ChildElements.Add(attribute);
            
            // Apply stereotypes
            RdbmsSchemaAnnotator.ApplyPrimaryKey(column, attribute);
            RdbmsSchemaAnnotator.ApplyColumnDetails(column, attribute);
            RdbmsSchemaAnnotator.ApplyTextConstraint(column, attribute);
            RdbmsSchemaAnnotator.ApplyDecimalConstraint(column, attribute);
            RdbmsSchemaAnnotator.ApplyDefaultConstraint(column, attribute);
            RdbmsSchemaAnnotator.ApplyComputedValue(column, attribute);
        }

        // Apply table stereotypes
        RdbmsSchemaAnnotator.ApplyTableDetails(config, table, classElement);
        
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
            ParentFolderId = parentFolderId, // Set parent folder for schema organization
            PackageId = package.Id,
            PackageName = package.Name,
            Stereotypes = [],
            Metadata = [],
            ChildElements = []
        };

        // Map columns to attributes
        foreach (var column in view.Columns)
        {
            var attribute = MapColumnToAttribute(column, view.Name, className, view.Schema, package, classElement.Id, deduplicationContext);
            classElement.ChildElements.Add(attribute);
            
            // Apply stereotypes (views don't have primary keys, defaults, or computed values)
            RdbmsSchemaAnnotator.ApplyColumnDetails(column, attribute);
            RdbmsSchemaAnnotator.ApplyTextConstraint(column, attribute);
            RdbmsSchemaAnnotator.ApplyDecimalConstraint(column, attribute);
        }

        // Apply view stereotypes
        RdbmsSchemaAnnotator.ApplyViewDetails(view, classElement);

        return classElement;
    }
    
    /// <summary>
    /// Creates a repository element for containing stored procedures/operations
    /// </summary>
    public static ElementPersistable CreateRepository(string repositoryName, string schemaFolderId, PackageModelPersistable package)
    {
        return new ElementPersistable
        {
            Id = Guid.NewGuid().ToString(),
            SpecializationType = Constants.SpecializationTypes.Repository.SpecializationType,
            SpecializationTypeId = Constants.SpecializationTypes.Repository.SpecializationTypeId,
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
    /// Maps stored procedure to element in Repository
    /// </summary>
    public static ElementPersistable MapStoredProcedureToElement(
        StoredProcedureSchema storedProc, 
        string? repositoryId, 
        PackageModelPersistable package, 
        DeduplicationContext? deduplicationContext = null)
    {
        return MapStoredProcedureToElement(storedProc, repositoryId, package, deduplicationContext, null);
    }

    /// <summary>
    /// Maps stored procedure to element in Repository with UserDefinedTable DataContract support
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
            ParentFolderId = repositoryId, // Stored procedures belong to repository
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
    /// Maps a stored procedure to an operation (Operation mode)
    /// </summary>
    public static ElementPersistable MapStoredProcedureToOperation(
        StoredProcedureSchema storedProc, 
        string repositoryId, 
        PackageModelPersistable package, 
        DeduplicationContext? deduplicationContext = null)
    {
        return MapStoredProcedureToOperation(storedProc, repositoryId, package, deduplicationContext, null);
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
    public static ElementPersistable CreateDataContractForStoredProcedure(StoredProcedureSchema storedProc, string schemaFolderId, string storedProcElementName, PackageModelPersistable package)
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

        // Map result set columns to attributes
        foreach (var resultColumn in storedProc.ResultSetColumns)
        {
            var attribute = MapResultSetColumnToAttribute(resultColumn, dataContract.Id, storedProc.Name, storedProc.Schema);
            dataContract.ChildElements.Add(attribute);
            
            // Apply stereotypes to result set columns
            RdbmsSchemaAnnotator.ApplyColumnDetails(ConvertToColumnSchema(resultColumn), attribute);
            RdbmsSchemaAnnotator.ApplyTextConstraint(ConvertToColumnSchema(resultColumn), attribute);
            RdbmsSchemaAnnotator.ApplyDecimalConstraint(ConvertToColumnSchema(resultColumn), attribute);
        }

        return dataContract;
    }

    /// <summary>
    /// Creates a data contract for UserDefinedTable type
    /// Follows similar pattern to CreateDataContractForStoredProcedure but for UDT columns
    /// </summary>
    public static ElementPersistable CreateDataContractForUserDefinedTable(UserDefinedTableTypeSchema udtSchema, string schemaFolderId, PackageModelPersistable package)
    {
        var dataContractName = ModelNamingUtilities.NormalizeUserDefinedTableName(udtSchema.Name);
        
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
            var attribute = MapUserDefinedTableColumnToAttribute(column, dataContract.Id, udtSchema.Name, udtSchema.Schema);
            dataContract.ChildElements.Add(attribute);
            
            // Apply stereotypes to UDT columns
            RdbmsSchemaAnnotator.ApplyColumnDetails(column, attribute);
            RdbmsSchemaAnnotator.ApplyTextConstraint(column, attribute);
            RdbmsSchemaAnnotator.ApplyDecimalConstraint(column, attribute);
        }

        return dataContract;
    }
    
    /// <summary>
    /// Creates an index element with proper mapping to attributes
    /// </summary>
    public static ElementPersistable CreateIndex(TableSchema table, IndexSchema index, string classId, PackageModelPersistable package)
    {
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
            ChildElements = [],
        };

        // Apply index stereotypes
        RdbmsSchemaAnnotator.ApplyIndexStereotype(indexElement, index);

        return indexElement;
    }

    /// <summary>
    /// Creates an index column element with mapping to the corresponding attribute
    /// </summary>
    public static ElementPersistable CreateIndexColumn(IndexColumnSchema indexColumn, string indexId, string? attributeId, PackageModelPersistable package)
    {
        BasicMappingModelPersistable? mapping = null;
        if (attributeId != null)
        {
            mapping = new BasicMappingModelPersistable
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
        }

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
            IsMapped = mapping != null,
            Mapping = mapping,
            ParentFolderId = indexId, // Index columns belong to their parent index
            PackageId = package.Id,
            PackageName = package.Name,
            Stereotypes = [],
            Metadata = [],
            ChildElements = []
        };

        // Apply index column stereotypes
        RdbmsSchemaAnnotator.ApplyIndexColumnStereotype(columnIndex, indexColumn);

        return columnIndex;
    }
    
    /// <summary>
    /// Gets or creates an association based on foreign key information
    /// Ported from original DatabaseSchemaToModelMapper.GetOrCreateAssociation
    /// </summary>
    public static AssociationPersistable? GetOrCreateAssociation(ForeignKeySchema foreignKey, TableSchema sourceTable, ElementPersistable sourceClass, PackageModelPersistable package)
    {
        var targetTableExternalRef = ModelNamingUtilities.GetTableExternalReference(foreignKey.ReferencedTableSchema, foreignKey.ReferencedTableName);
        
        // Find target class by ExternalReference first, then by name
        var targetClass = package.Classes.FirstOrDefault(c => 
            c.ExternalReference == targetTableExternalRef && c.SpecializationType == ClassModel.SpecializationType) ??
            package.Classes.FirstOrDefault(c => 
                c.Name.Equals(ModelNamingUtilities.GetEntityName(foreignKey.ReferencedTableName, EntityNameConvention.SingularEntity, foreignKey.ReferencedTableSchema, null), StringComparison.OrdinalIgnoreCase) && 
                c.SpecializationType == ClassModel.SpecializationType);

        if (targetClass == null)
        {
            return null; // Target class not found, skip association creation
        }

        // Generate target name based on foreign key column naming
        string targetName;
        var singularTableName = foreignKey.ReferencedTableName.Singularize();
        var firstColumnName = foreignKey.Columns.First().Name;

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

        // Generate external reference for foreign key
        var fkExternalRef = ModelNamingUtilities.GetForeignKeyExternalReference(sourceTable.Schema, sourceTable.Name, foreignKey.Name);

        // Check if association already exists by foreign key external reference
        var association = package.Associations?.FirstOrDefault(a => a.ExternalReference == fkExternalRef);

        if (association == null)
        {
            // Determine if this is a one-to-one relationship (FK columns are all primary keys)
            var sourcePKColumns = sourceTable.Columns.Where(c => c.IsPrimaryKey).Select(c => c.Name).ToHashSet();
            var fkColumnNames = foreignKey.Columns.Select(c => c.Name).ToHashSet();
            var isOneToOne = sourcePKColumns.Count == fkColumnNames.Count && 
                           fkColumnNames.All(fk => sourcePKColumns.Contains(fk));

            // Check if any FK columns are nullable to determine association nullability
            var isNullable = foreignKey.Columns.Any(fkCol => 
                sourceTable.Columns.Any(col => col.Name == fkCol.Name && col.IsNullable));

            // Avoid naming conflicts
            var finalTargetName = targetName.Equals(sourceTable.Name.Singularize(), StringComparison.OrdinalIgnoreCase) 
                ? $"{targetName}Reference" 
                : targetName;

            var associationId = Guid.NewGuid().ToString();
            association = new AssociationPersistable
            {
                Id = associationId,
                ExternalReference = fkExternalRef,
                AssociationTypeId = Constants.SpecializationTypes.Association.SpecializationTypeId,
                AssociationType = Constants.SpecializationTypes.Association.SpecializationType,
                TargetEnd = new AssociationEndPersistable
                {
                    Id = associationId, // Keep same as association ID for target end
                    Name = finalTargetName,
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
            if (!SameAssociationExistsWithReverseOwnership(package.Associations?.ToList(), association))
            {
                package.Associations ??= new List<AssociationPersistable>();
                package.Associations.Add(association);
            }
        }

        return association;
    }
    
    /// <summary>
    /// Creates a folder element for schema organization
    /// </summary>
    public static ElementPersistable CreateSchemaFolder(string schemaName, PackageModelPersistable package)
    {
        var folderSchemaName = ModelNamingUtilities.NormalizeSchemaName(schemaName);
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

        RdbmsSchemaAnnotator.AddSchemaStereotype(folder, schemaName);
        
        return folder;
    }

    private static ElementPersistable MapColumnToAttribute(
        ColumnSchema column, 
        string tableName, 
        string className, 
        string schema, 
        PackageModelPersistable package,
        string? parentClassId = null, 
        DeduplicationContext? deduplicationContext = null)
    {
        var attributeName = ModelNamingUtilities.GetAttributeName(column.Name, tableName, className, schema, deduplicationContext);
        
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
            ParentFolderId = parentClassId, // Attributes belong to their parent class
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
    /// Maps a stored procedure parameter to a stored procedure element
    /// </summary>
    private static ElementPersistable MapStoredProcParameterToElement(StoredProcedureParameterSchema parameter, string storedProcId, PackageModelPersistable package)
    {
        return MapStoredProcParameterToElement(parameter, storedProcId, package, null);
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
    /// Maps a stored procedure parameter to an operation parameter
    /// </summary>
    private static ElementPersistable MapStoredProcParameterToOperation(StoredProcedureParameterSchema parameter, string operationId, PackageModelPersistable package)
    {
        return MapStoredProcParameterToOperation(parameter, operationId, package, null);
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
    private static ElementPersistable MapResultSetColumnToAttribute(ResultSetColumnSchema resultColumn, string dataContractId, string procName, string schema)
    {
        var attributeName = ModelNamingUtilities.NormalizeColumnName(resultColumn.Name, null); // No table name for result sets
        
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
    private static ElementPersistable MapUserDefinedTableColumnToAttribute(ColumnSchema column, string dataContractId, string udtName, string schema)
    {
        var attributeName = ModelNamingUtilities.NormalizeColumnName(column.Name, null); // No table name for UDT columns
        
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

    /// <summary>
    /// Converts ResultSetColumnSchema to ColumnSchema for stereotype application
    /// This allows reuse of existing stereotype logic
    /// </summary>
    private static ColumnSchema ConvertToColumnSchema(ResultSetColumnSchema resultColumn)
    {
        return new ColumnSchema
        {
            Name = resultColumn.Name,
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
}
