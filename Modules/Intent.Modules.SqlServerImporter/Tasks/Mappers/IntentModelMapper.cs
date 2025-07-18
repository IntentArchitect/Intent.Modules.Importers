using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Intent.RelationalDbSchemaImporter.Contracts.Schema;
using Intent.RelationalDbSchemaImporter.Contracts.Enums;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Modules.SqlServerImporter.Tasks.Models;
using Intent.Modules.Common.Templates;
using Intent.Modelers.Domain.Api;

namespace Intent.Modules.SqlServerImporter.Tasks.Mappers;

public class IntentModelMapper
{
    private readonly TypeReferenceMapper _typeReferenceMapper;
    
    // Reserved C# keywords
    private static readonly HashSet<string> ReservedWords = new(StringComparer.OrdinalIgnoreCase)
    {
        "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked", "class", "const", "continue", "decimal", "default", "delegate", "do",
        "double", "else", "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for", "foreach", "goto", "if", "implicit", "in", "int", 
        "interface", "internal", "is", "lock", "long", "namespace", "new", "null", "object", "operator", "out", "override", "params", "private", "protected", "public",
        "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof", "stackalloc", "static", "string", "struct", "switch", "this", "throw", "true", "try",
        "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort", "using", "using static", "virtual", "void", "volatile", "while"
    };

    public IntentModelMapper()
    {
        _typeReferenceMapper = new TypeReferenceMapper();
    }

    public ElementPersistable MapTableToClass(TableSchema table, ImportConfiguration config, string? parentFolderId = null, DeduplicationContext? deduplicationContext = null)
    {
        var className = GetEntityName(table.Name, config.EntityNameConvention, table.Schema, deduplicationContext);
        
        var classElement = new ElementPersistable
        {
            Id = Guid.NewGuid().ToString(),
            Name = className,
            SpecializationType = ClassModel.SpecializationType,
            SpecializationTypeId = ClassModel.SpecializationTypeId,
            ParentFolderId = parentFolderId, // Set parent folder for schema organization
            ChildElements = new List<ElementPersistable>(),
            Stereotypes = new List<StereotypePersistable>(),
            ExternalReference = GetTableExternalReference(table.Name, table.Schema)
        };

        // Map columns to attributes
        foreach (var column in table.Columns)
        {
            var attribute = MapColumnToAttribute(column, table.Name, className, table.Schema, classElement.Id, deduplicationContext);
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
            var triggerElement = MapTriggerToElement(trigger, table.Name, table.Schema, classElement.Id);
            classElement.ChildElements.Add(triggerElement);
        }

        return classElement;
    }

    public ElementPersistable MapViewToClass(ViewSchema view, ImportConfiguration config, string? parentFolderId = null, DeduplicationContext? deduplicationContext = null)
    {
        var className = GetViewName(view.Name, config.EntityNameConvention, view.Schema, deduplicationContext);
        
        var classElement = new ElementPersistable
        {
            Id = Guid.NewGuid().ToString(),
            Name = className,
            SpecializationType = ClassModel.SpecializationType,
            SpecializationTypeId = ClassModel.SpecializationTypeId,
            ParentFolderId = parentFolderId, // Set parent folder for schema organization
            ChildElements = new List<ElementPersistable>(),
            Stereotypes = new List<StereotypePersistable>(),
            ExternalReference = GetViewExternalReference(view.Name, view.Schema)
        };

        // Map columns to attributes
        foreach (var column in view.Columns)
        {
            var attribute = MapColumnToAttribute(column, view.Name, className, view.Schema, classElement.Id, deduplicationContext);
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

    private ElementPersistable MapColumnToAttribute(ColumnSchema column, string tableName, string className, string schema, string? parentClassId = null, DeduplicationContext? deduplicationContext = null)
    {
        var attributeName = GetAttributeName(column.Name, tableName, className, schema, deduplicationContext);
        
        return new ElementPersistable
        {
            Id = Guid.NewGuid().ToString(),
            Name = attributeName,
            SpecializationType = AttributeModel.SpecializationType,
            SpecializationTypeId = AttributeModel.SpecializationTypeId,
            ParentFolderId = parentClassId, // Attributes belong to their parent class
            TypeReference = _typeReferenceMapper.MapColumnTypeToTypeReference(column),
            ChildElements = new List<ElementPersistable>(),
            Stereotypes = new List<StereotypePersistable>(),
            ExternalReference = GetColumnExternalReference(column.Name, tableName, schema)
        };
    }

    private string GetEntityName(string tableName, EntityNameConvention convention, string schema, DeduplicationContext? deduplicationContext)
    {
        var normalized = convention switch
        {
            EntityNameConvention.MatchTable => NormalizeTableName(tableName),
            EntityNameConvention.SingularEntity => NormalizeTableName(tableName.Singularize()),
            _ => NormalizeTableName(tableName)
        };
        return deduplicationContext?.DeduplicateTable(normalized, schema) ?? normalized;
    }

    private string GetViewName(string viewName, EntityNameConvention convention, string schema, DeduplicationContext? deduplicationContext)
    {
        var normalized = convention switch
        {
            EntityNameConvention.MatchTable => NormalizeTableName(viewName),
            EntityNameConvention.SingularEntity => NormalizeTableName(viewName.Singularize()),
            _ => NormalizeTableName(viewName)
        };
        return deduplicationContext?.DeduplicateView(normalized, schema) ?? normalized;
    }

    private string GetAttributeName(string columnName, string? tableName, string className, string schema, DeduplicationContext? deduplicationContext)
    {
        // Normalize column name
        var normalized = NormalizeColumnName(columnName, tableName);
        return deduplicationContext?.DeduplicateColumn(normalized, className, schema) ?? normalized;
    }

    private string GetStoredProcedureName(string procName, string schema, DeduplicationContext? deduplicationContext)
    {
        // Remove schema prefix if present and convert to PascalCase
        var name = procName.Contains('.') ? procName.Split('.').Last() : procName;
        var normalized = NormalizeStoredProcName(name);
        return deduplicationContext?.DeduplicateStoredProcedure(normalized, schema) ?? normalized;
    }

    private string GetParameterName(string paramName)
    {
        // Remove @ prefix if present and convert to camelCase
        var name = paramName.StartsWith("@") ? paramName.Substring(1) : paramName;
        return name.ToCamelCase();
    }

    #region Name Normalization

    /// <summary>
    /// Converts database identifier to valid C# identifier following C# naming conventions
    /// </summary>
    public static string ToCSharpIdentifier(string identifier, string prefixValue = "Db")
    {
        if (string.IsNullOrWhiteSpace(identifier))
        {
            return string.Empty;
        }

        // Replace common symbols
        identifier = identifier
            .Replace("#", "Sharp")
            .Replace("&", "And");

        var asCharArray = identifier.ToCharArray();
        for (var i = 0; i < asCharArray.Length; i++)
        {
            // Underscore character conversion to space for processing
            if (asCharArray[i] == '_')
            {
                asCharArray[i] = ' ';
                continue;
            }

            switch (char.GetUnicodeCategory(asCharArray[i]))
            {
                case UnicodeCategory.DecimalDigitNumber:
                case UnicodeCategory.LetterNumber:
                case UnicodeCategory.LowercaseLetter:
                case UnicodeCategory.ModifierLetter:
                case UnicodeCategory.OtherLetter:
                case UnicodeCategory.TitlecaseLetter:
                case UnicodeCategory.UppercaseLetter:
                case UnicodeCategory.Format:
                    break;
                default:
                    asCharArray[i] = ' ';
                    break;
            }
        }

        identifier = new string(asCharArray);

        // Replace double spaces
        while (identifier.Contains("  "))
        {
            identifier = identifier.Replace("  ", " ");
        }

        // Convert to PascalCase
        identifier = string.Concat(identifier
            .Split(' ')
            .Where(element => !string.IsNullOrWhiteSpace(element))
            .Select((element, index) => index == 0
                ? element
                : element.ToPascalCase()));

        // Ensure identifier starts with letter
        if (char.IsNumber(identifier[0]))
        {
            identifier = $"{prefixValue}{identifier}";
        }

        // Handle reserved words
        if (ReservedWords.Contains(identifier))
        {
            identifier = $"{prefixValue}{identifier}";
        }

        return identifier;
    }

    private static string NormalizeTableName(string tableName)
    {
        var normalized = tableName.RemovePrefix("tbl");
        normalized = ToCSharpIdentifier(normalized, "Db");
        normalized = normalized.Substring(0, 1).ToUpper() + normalized.Substring(1);
        return normalized;
    }

    private static string NormalizeColumnName(string colName, string? tableOrViewName)
    {
        var normalized = colName != tableOrViewName ? colName : colName + "Value";
        normalized = ToCSharpIdentifier(normalized, "db");
        normalized = normalized.RemovePrefix("col").RemovePrefix("pk");
        normalized = normalized.Substring(0, 1).ToUpper() + normalized.Substring(1);

        if (normalized.EndsWith("ID"))
        {
            normalized = normalized.RemoveSuffix("ID") + "Id";
        }

        return normalized;
    }

    private static string NormalizeStoredProcName(string storeProcName)
    {
        var normalized = ToCSharpIdentifier(storeProcName);
        normalized = normalized.RemovePrefix("prc")
            .RemovePrefix("Prc")
            .RemovePrefix("proc");
        normalized = normalized.Substring(0, 1).ToUpper() + normalized.Substring(1);
        return normalized;
    }

    private static string NormalizeSchemaName(string schemaName)
    {
        var normalized = schemaName;
        return normalized.Substring(0, 1).ToUpper() + normalized.Substring(1);
    }

    #endregion

    #region ExternalReference Generation

    /// <summary>
    /// Generates ExternalReference for table following [schema].[tablename] pattern
    /// </summary>
    public static string GetTableExternalReference(string tableName, string schema)
    {
        return $"[{schema}].[{tableName}]";
    }

    /// <summary>
    /// Generates ExternalReference for view following [schema].[viewname] pattern
    /// </summary>
    public static string GetViewExternalReference(string viewName, string schema)
    {
        return $"[{schema}].[{viewName}]";
    }

    /// <summary>
    /// Generates ExternalReference for column following [schema].[tablename].[columnname] pattern
    /// </summary>
    public static string GetColumnExternalReference(string columnName, string tableName, string schema)
    {
        return $"[{schema}].[{tableName}].[{columnName}]";
    }

    /// <summary>
    /// Generates ExternalReference for stored procedure following [schema].[procname] pattern
    /// </summary>
    public static string GetStoredProcedureExternalReference(string procName, string schema)
    {
        return $"stored-procedure:[{schema.ToLower()}].[{procName.ToLower()}]";
    }

    /// <summary>
    /// Generates ExternalReference for foreign key following [schema].[tablename].[fkname] pattern
    /// </summary>
    private static string GetForeignKeyExternalReference(string fkName, string tableName, string schema)
    {
        return $"[{schema}].[{tableName}].[{fkName}]";
    }

    public static string GetTriggerExternalReference(string triggerName, string tableName, string schema)
    {
        return $"trigger:[{schema.ToLower()}].[{tableName.ToLower()}].[{triggerName.ToLower()}]";
    }

    #endregion

    #region Stored Procedure and Repository Support

    /// <summary>
    /// Creates a repository element for containing stored procedures/operations
    /// </summary>
    public ElementPersistable CreateRepository(string repositoryName, string schemaFolderId)
    {
        return new ElementPersistable
        {
            Id = Guid.NewGuid().ToString(),
            Name = repositoryName,
            SpecializationType = "Repository",
            SpecializationTypeId = "96ffceb2-a70a-4b69-869b-0df436c470c3", // Repository specialization type ID
            ParentFolderId = schemaFolderId, // Repositories belong to schema folders
            ChildElements = new List<ElementPersistable>(),
            Stereotypes = new List<StereotypePersistable>()
        };
    }

    /// <summary>
    /// Maps a stored procedure to an element (Element mode)
    /// </summary>
    public ElementPersistable MapStoredProcedureToElement(StoredProcedureSchema storedProc, string? repositoryId, ImportConfiguration config, DeduplicationContext? deduplicationContext = null)
    {
        var procName = GetStoredProcedureName(storedProc.Name, storedProc.Schema, deduplicationContext);

        var procElement = new ElementPersistable
        {
            Id = Guid.NewGuid().ToString(),
            Name = procName,
            SpecializationType = "Stored Procedure",
            SpecializationTypeId = "575edd35-9438-406d-b0a7-b99d6f29b560", // Stored Procedure specialization type ID
            ParentFolderId = repositoryId, // Stored procedures belong to repository
            ChildElements = new List<ElementPersistable>(),
            Stereotypes = new List<StereotypePersistable>(),
            ExternalReference = GetStoredProcedureExternalReference(storedProc.Name, storedProc.Schema)
        };

        // Map parameters
        foreach (var parameter in storedProc.Parameters)
        {
            var paramElement = MapStoredProcParameterToElement(parameter, procElement.Id, storedProc.Schema);
            procElement.ChildElements.Add(paramElement);
        }

        return procElement;
    }

    /// <summary>
    /// Maps a stored procedure to an operation (Operation mode)
    /// </summary>
    public ElementPersistable MapStoredProcedureToOperation(StoredProcedureSchema storedProc, string repositoryId, ImportConfiguration config, DeduplicationContext? deduplicationContext = null)
    {
        var procName = GetStoredProcedureName(storedProc.Name, storedProc.Schema, deduplicationContext);

        var operationElement = new ElementPersistable
        {
            Id = Guid.NewGuid().ToString(),
            Name = procName,
            SpecializationType = "Operation",
            SpecializationTypeId = "e030c97a-e066-40a7-8188-808c275df3cb", // Operation specialization type ID
            ParentFolderId = repositoryId, // Operations belong to repository
            ChildElements = new List<ElementPersistable>(),
            Stereotypes = new List<StereotypePersistable>(),
            ExternalReference = GetStoredProcedureExternalReference(storedProc.Name, storedProc.Schema)
        };

        // Map parameters
        foreach (var parameter in storedProc.Parameters)
        {
            var paramElement = MapStoredProcParameterToParameter(parameter, operationElement.Id, storedProc.Schema);
            operationElement.ChildElements.Add(paramElement);
        }

        // Set return type if stored procedure has result set
        if (storedProc.ResultSetColumns.Count > 0)
        {
            operationElement.TypeReference = _typeReferenceMapper.CreateStoredProcedureReturnTypeReference(null, true);
        }

        return operationElement;
    }

    /// <summary>
    /// Maps a stored procedure parameter to a stored procedure parameter element
    /// </summary>
    private ElementPersistable MapStoredProcParameterToElement(StoredProcedureParameterSchema parameter, string storedProcId, string schema)
    {
        var paramName = GetParameterName(parameter.Name);

        return new ElementPersistable
        {
            Id = Guid.NewGuid().ToString(),
            Name = paramName,
            SpecializationType = "Stored Procedure Parameter",
            SpecializationTypeId = "5823b192-eb03-47c8-90d8-5501c922e9a5", // Stored Procedure Parameter specialization type ID
            ParentFolderId = storedProcId, // Parameters belong to stored procedure
            TypeReference = _typeReferenceMapper.MapStoredProcedureParameterTypeToTypeReference(parameter),
            ChildElements = new List<ElementPersistable>(),
            Stereotypes = new List<StereotypePersistable>()
        };
    }

    /// <summary>
    /// Maps a stored procedure parameter to an operation parameter
    /// </summary>
    private ElementPersistable MapStoredProcParameterToParameter(StoredProcedureParameterSchema parameter, string operationId, string schema)
    {
        var paramName = GetParameterName(parameter.Name);

        return new ElementPersistable
        {
            Id = Guid.NewGuid().ToString(),
            Name = paramName,
            SpecializationType = "Parameter",
            SpecializationTypeId = "00208d20-469d-41cb-8501-768fd5eb796b", // Parameter specialization type ID
            ParentFolderId = operationId, // Parameters belong to operation
            TypeReference = _typeReferenceMapper.MapStoredProcedureParameterTypeToTypeReference(parameter),
            ChildElements = new List<ElementPersistable>(),
            Stereotypes = new List<StereotypePersistable>()
        };
    }

    #endregion

    #region Index and Mapping Support

    /// <summary>
    /// Creates an index element with proper mapping to attributes
    /// </summary>
    public ElementPersistable CreateIndex(IndexSchema index, string classId, PackageModelPersistable package)
    {
        var indexElement = new ElementPersistable
        {
            Id = Guid.NewGuid().ToString(),
            Name = index.Name,
            SpecializationType = "Index",
            SpecializationTypeId = "436e3afe-b4ef-481c-b803-0d1e7d263561", // Index specialization type ID
            ParentFolderId = classId, // Indexes belong to their parent class
            ChildElements = new List<ElementPersistable>(),
            Stereotypes = new List<StereotypePersistable>(),
            IsMapped = true,
            Mapping = new BasicMappingModelPersistable
            {
                ApplicationId = package.ApplicationId,
                MappingSettingsId = "30f4278f-1d74-4e7e-bfdb-39c8e120f24c", // Column mapping settings ID
                MetadataId = "6ab29b31-27af-4f56-a67c-986d82097d63", // Domain metadata ID
                AutoSyncTypeReference = false,
                Path = new List<MappedPathTargetPersistable>
                {
                    new()
                    {
                        Id = classId,
                        Name = GetClassNameById(classId, package),
                        Type = "Element",
                        Specialization = "Class"
                    }
                }
            },
            PackageId = package.Id,
            PackageName = package.Name
        };

        // Apply index stereotypes
        ApplyIndexStereotype(indexElement, index);

        return indexElement;
    }

    /// <summary>
    /// Creates an index column element with mapping to the corresponding attribute
    /// </summary>
    public ElementPersistable CreateIndexColumn(IndexColumnSchema indexColumn, string indexId, string? attributeId, PackageModelPersistable package)
    {
        BasicMappingModelPersistable? mapping = null;
        if (attributeId != null)
        {
            mapping = new BasicMappingModelPersistable
            {
                MappingSettingsId = "30f4278f-1d74-4e7e-bfdb-39c8e120f24c", // Column mapping settings ID
                MetadataId = "6ab29b31-27af-4f56-a67c-986d82097d63", // Domain metadata ID
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
            Name = indexColumn.Name,
            SpecializationType = "Index Column",
            SpecializationTypeId = "c5ba925d-5c08-4809-a848-585a0cd4ddd3", // Index Column specialization type ID
            IsMapped = mapping != null,
            Mapping = mapping,
            ParentFolderId = indexId, // Index columns belong to their parent index
            PackageId = package.Id,
            PackageName = package.Name,
            ChildElements = new List<ElementPersistable>(),
            Stereotypes = new List<StereotypePersistable>()
        };

        // Apply index column stereotypes
        ApplyIndexColumnStereotype(columnIndex, indexColumn);

        return columnIndex;
    }

    private void ApplyIndexStereotype(ElementPersistable indexElement, IndexSchema index)
    {
        var indexStereotype = new StereotypePersistable
        {
            Name = "Index Settings",
            DefinitionId = Constants.Stereotypes.Rdbms.Index.Settings.DefinitionId,
            DefinitionPackageId = Constants.Packages.Rdbms.DefinitionPackageId,
            DefinitionPackageName = Constants.Packages.Rdbms.DefinitionPackageName,
            AddedByDefault = true,
            Properties = new List<StereotypePropertyPersistable>
            {
                new()
                {
                    DefinitionId = Constants.Stereotypes.Rdbms.Index.Settings.PropertyId.UseDefaultName,
                    Name = Constants.Stereotypes.Rdbms.Index.Settings.PropertyId.UseDefaultNameName,
                    Value = "false"
                },
                new()
                {
                    DefinitionId = Constants.Stereotypes.Rdbms.Index.Settings.PropertyId.Unique,
                    Name = Constants.Stereotypes.Rdbms.Index.Settings.PropertyId.UniqueName,
                    Value = index.IsUnique.ToString().ToLower()
                },
                new()
                {
                    DefinitionId = Constants.Stereotypes.Rdbms.Index.Settings.PropertyId.Filter,
                    Name = Constants.Stereotypes.Rdbms.Index.Settings.PropertyId.FilterName,
                    Value = "Default"
                }
            }
        };

        indexElement.Stereotypes.Add(indexStereotype);
    }

    private void ApplyIndexColumnStereotype(ElementPersistable columnIndex, IndexColumnSchema indexColumn)
    {
        var indexColumnStereotype = new StereotypePersistable
        {
            Name = "Index Column Settings",
            DefinitionId = Constants.Stereotypes.Rdbms.Index.IndexColumn.Settings.DefinitionId,
            DefinitionPackageId = Constants.Packages.Rdbms.DefinitionPackageId,
            DefinitionPackageName = Constants.Packages.Rdbms.DefinitionPackageName,
            AddedByDefault = true,
            Properties = new List<StereotypePropertyPersistable>
            {
                new()
                {
                    DefinitionId = Constants.Stereotypes.Rdbms.Index.IndexColumn.Settings.PropertyId.Type,
                    Name = Constants.Stereotypes.Rdbms.Index.IndexColumn.Settings.PropertyId.TypeName,
                    Value = indexColumn.IsIncluded ? "Included" : "Key"
                },
                new()
                {
                    DefinitionId = Constants.Stereotypes.Rdbms.Index.IndexColumn.Settings.PropertyId.SortDirection,
                    Name = Constants.Stereotypes.Rdbms.Index.IndexColumn.Settings.PropertyId.SortDirectionName,
                    Value = indexColumn.IsDescending ? "Descending" : "Ascending"
                }
            }
        };

        columnIndex.Stereotypes.Add(indexColumnStereotype);
    }

    private string GetClassNameById(string classId, PackageModelPersistable package)
    {
        var element = package.Classes.FirstOrDefault(c => c.Id == classId);
        return element?.Name ?? "Unknown";
    }

    private string GetAttributeNameById(string attributeId, PackageModelPersistable package)
    {
        foreach (var classElement in package.Classes)
        {
            var attribute = classElement.ChildElements?.FirstOrDefault(c => c.Id == attributeId);
            if (attribute != null)
                return attribute.Name;
        }
        return "Unknown";
    }

    #endregion

    #region Foreign Key and Association Support

    /// <summary>
    /// Gets or creates an association based on foreign key information
    /// Ported from original DatabaseSchemaToModelMapper.GetOrCreateAssociation
    /// </summary>
    public AssociationPersistable? GetOrCreateAssociation(ForeignKeySchema foreignKey, TableSchema sourceTable, ElementPersistable sourceClass, PackageModelPersistable package)
    {
        var targetTableExternalRef = GetTableExternalReference(foreignKey.ReferencedTableName, foreignKey.ReferencedTableSchema);
        
        // Find target class by ExternalReference first, then by name
        var targetClass = package.Classes.FirstOrDefault(c => 
            c.ExternalReference == targetTableExternalRef && c.SpecializationType == "Class") ??
            package.Classes.FirstOrDefault(c => 
                c.Name.Equals(GetEntityName(foreignKey.ReferencedTableName, EntityNameConvention.SingularEntity, foreignKey.ReferencedTableSchema, null), StringComparison.OrdinalIgnoreCase) && 
                c.SpecializationType == "Class");

        if (targetClass == null)
        {
            return null; // Target class not found, skip association creation
        }

        // Generate target name based on foreign key column naming
        string targetName;
        var singularTableName = foreignKey.ReferencedTableName.Singularize();
        var firstColumnName = foreignKey.Columns.First().Name;

        // Determine association target name based on column naming patterns
        if (firstColumnName.IndexOf(singularTableName, StringComparison.OrdinalIgnoreCase) == 0)
        {
            targetName = singularTableName;
        }
        else if (firstColumnName.IndexOf(foreignKey.ReferencedTableName, StringComparison.OrdinalIgnoreCase) == -1)
        {
            targetName = firstColumnName.Replace("ID", "", StringComparison.OrdinalIgnoreCase) + foreignKey.ReferencedTableName;
        }
        else if (firstColumnName.IndexOf(foreignKey.ReferencedTableName, StringComparison.OrdinalIgnoreCase) == 0)
        {
            targetName = singularTableName;
        }
        else
        {
            var index = firstColumnName.IndexOf(foreignKey.ReferencedTableName, StringComparison.OrdinalIgnoreCase);
            targetName = firstColumnName.Substring(0, index + foreignKey.ReferencedTableName.Length);
        }

        // Generate external reference for foreign key
        var fkExternalRef = GetForeignKeyExternalReference(foreignKey.Name, sourceTable.Name, sourceTable.Schema);

        // Check if association already exists
        var association = package.Associations?.FirstOrDefault(a => a.ExternalReference == fkExternalRef);

        if (association == null)
        {
            // Check for existing association by target/source types and target name
            var existingAssociations = package.Associations?.Where(a =>
                a.TargetEnd.TypeReference?.TypeId == targetClass.Id &&
                a.SourceEnd.TypeReference?.TypeId == sourceClass.Id) ?? Enumerable.Empty<AssociationPersistable>();

            if (existingAssociations.Count() == 1)
            {
                association = existingAssociations.First();
            }
            else
            {
                association = existingAssociations.FirstOrDefault(a => a.TargetEnd.Name == targetName);
            }
        }

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
                AssociationTypeId = "eaf9ed4e-0b61-4ac1-ba88-09f912c12087", // Association specialization type ID
                AssociationType = "Association",
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
                    Stereotypes = new List<StereotypePersistable>()
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
                    Stereotypes = new List<StereotypePersistable>()
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

    /// <summary>
    /// Determines if a column should get a Foreign Key stereotype
    /// Excludes columns that are both Primary Key and Foreign Key (one-to-one relationships)
    /// </summary>
    public static bool ShouldApplyForeignKeyStereotype(ColumnSchema column, ForeignKeySchema foreignKey)
    {
        // Don't apply FK stereotype if the column is both PK and FK (one-to-one relationship)
        if (column.IsPrimaryKey && foreignKey.Columns.Any(fk => fk.Name == column.Name))
        {
            return false;
        }

        return true;
    }

    #endregion

    #region Folder and Schema Support

    /// <summary>
    /// Creates a folder element for schema organization
    /// </summary>
    public ElementPersistable CreateSchemaFolder(string schemaName, string packageId)
    {
        return new ElementPersistable
        {
            Id = Guid.NewGuid().ToString(),
            Name = NormalizeSchemaName(schemaName),
            SpecializationType = "Folder",
            SpecializationTypeId = "4d95d53a-8855-4f35-9820-3106413fec04", // Generic Folder
            ParentFolderId = packageId,
            ExternalReference = $"schema:[{schemaName.ToLower()}]"
        };
    }

    private ElementPersistable MapTriggerToElement(TriggerSchema trigger, string tableName, string schema, string parentClassId)
    {
        var triggerName = trigger.Name; // Names are typically unique and descriptive
        
        var triggerElement = new ElementPersistable
        {
            Id = Guid.NewGuid().ToString(),
            Name = triggerName,
            SpecializationType = "Trigger", // Using a descriptive name for the element type
            SpecializationTypeId = "5b7b5e77-e627-464b-a157-6d01f2042641", // Replace with a valid GUID for "Trigger" element
            ParentFolderId = parentClassId,
            Stereotypes = new List<StereotypePersistable>(),
            ExternalReference = GetTriggerExternalReference(trigger.Name, tableName, schema)
        };
        
        // Optionally, apply stereotypes to the trigger if it needs to hold more metadata
        // RdbmsSchemaAnnotator.ApplyTriggerDetails(trigger, triggerElement);
        
        return triggerElement;
    }

    #endregion
}
