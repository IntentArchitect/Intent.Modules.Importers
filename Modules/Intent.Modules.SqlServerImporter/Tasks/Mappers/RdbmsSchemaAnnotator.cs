using System;
using System.Collections.Generic;
using System.Linq;
using Intent.RelationalDbSchemaImporter.Contracts.Enums;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.SqlServerImporter.Tasks.Models;
using Intent.RelationalDbSchemaImporter.Contracts.DbSchema;

namespace Intent.Modules.SqlServerImporter.Tasks.Mappers;

internal static class RdbmsSchemaAnnotator
{
    public static void ApplyTableDetails(ImportConfiguration config, TableSchema table, ElementPersistable @class)
    {
        if (!RequiresTableStereoType(config, table, @class))
        {
            return;
        }

        var stereotype = @class.GetOrCreateStereotype(Constants.Stereotypes.Rdbms.Table.DefinitionId, InitTableStereotype);
        //For now always set this in case generated table names don't match the generated names due to things like pluralization
        stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.Table.PropertyId.Name).Value = table.Name;
        return;
        
        static bool RequiresTableStereoType(ImportConfiguration config, TableSchema table, ElementPersistable @class)
        {
            return config.TableStereotype switch
            {
                TableStereotype.Always => true,
                TableStereotype.WhenDifferent when config.EntityNameConvention is EntityNameConvention.MatchTable => @class.Name != table.Name,
                TableStereotype.WhenDifferent when config.EntityNameConvention is EntityNameConvention.SingularEntity => @class.Name.Singularize() != table.Name,
                _ => false
            };
        }

        static void InitTableStereotype(StereotypePersistable stereotype)
        {
            stereotype.Name = Constants.Stereotypes.Rdbms.Table.Name;
            stereotype.DefinitionPackageId = Constants.Packages.Rdbms.DefinitionPackageId;
            stereotype.DefinitionPackageName = Constants.Packages.Rdbms.DefinitionPackageName;
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.Table.PropertyId.Name, prop => 
            {
                prop.Name = Constants.Stereotypes.Rdbms.Table.PropertyId.NameName;
            });
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.Table.PropertyId.Schema, prop => 
            {
                prop.Name = Constants.Stereotypes.Rdbms.Table.PropertyId.SchemaName;
            });
        }
    }

    public static void ApplyViewDetails(ViewSchema view, ElementPersistable @class)
    {
        var stereotype = @class.GetOrCreateStereotype(Constants.Stereotypes.Rdbms.View.DefinitionId, InitViewStereotype);
        if (view.Name != @class.Name)
        {
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.View.PropertyId.Name).Value = view.Name;
        }

        return;

        static void InitViewStereotype(StereotypePersistable stereotype)
        {
            stereotype.Name = Constants.Stereotypes.Rdbms.View.Name;
            stereotype.DefinitionPackageId = Constants.Packages.Rdbms.DefinitionPackageId;
            stereotype.DefinitionPackageName = Constants.Packages.Rdbms.DefinitionPackageName;
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.View.PropertyId.Name, prop => 
            {
                prop.Name = Constants.Stereotypes.Rdbms.View.PropertyId.NameName;
            });
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.View.PropertyId.Schema, prop => 
            {
                prop.Name = Constants.Stereotypes.Rdbms.View.PropertyId.SchemaName;
            });
        }
    }

    public static void ApplyPrimaryKey(ColumnSchema column, ElementPersistable attribute)
    {
        if (!column.IsPrimaryKey)
        {
            return;
        }

        var stereotype = attribute.GetOrCreateStereotype(Constants.Stereotypes.Rdbms.PrimaryKey.DefinitionId, InitPrimaryKeyStereotype);
        stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.PrimaryKey.PropertyId.DataSource).Value = GetDataSourceValue(column);
        return;

        static void InitPrimaryKeyStereotype(StereotypePersistable stereotype)
        {
            stereotype.Name = Constants.Stereotypes.Rdbms.PrimaryKey.Name;
            stereotype.DefinitionPackageId = Constants.Packages.Rdbms.DefinitionPackageId;
            stereotype.DefinitionPackageName = Constants.Packages.Rdbms.DefinitionPackageName;
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.PrimaryKey.PropertyId.DataSource, prop => 
            {
                prop.Name = Constants.Stereotypes.Rdbms.PrimaryKey.PropertyId.DataSourceName;
            });
        }
        
        static string GetDataSourceValue(ColumnSchema column)
        {
            if (column.IsIdentity)
            {
                return "Auto-generated";
            }

            return "Default";
        }
    }

    public static void ApplyColumnDetails(ColumnSchema column, ElementPersistable attribute)
    {
        if (column.Name == attribute.Name &&
            IsImplicitColumnType(column, attribute))
        {
            return;
        }

        var stereotype = attribute.GetOrCreateStereotype(Constants.Stereotypes.Rdbms.Column.DefinitionId, InitColumnStereotype);

        if (column.Name != attribute.Name)
        {
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.Column.PropertyId.Name).Value = column.Name;
        }

        if (!IsImplicitColumnType(column, attribute))
        {
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.Column.PropertyId.Type).Value = GetColumnTypeString(column);
        }

        return;

        static void InitColumnStereotype(StereotypePersistable stereotype)
        {
            stereotype.Name = Constants.Stereotypes.Rdbms.Column.Name;
            stereotype.DefinitionPackageId = Constants.Packages.Rdbms.DefinitionPackageId;
            stereotype.DefinitionPackageName = Constants.Packages.Rdbms.DefinitionPackageName;
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.Column.PropertyId.Name, prop => 
            {
                prop.Name = Constants.Stereotypes.Rdbms.Column.PropertyId.NameName;
            });
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.Column.PropertyId.Type, prop => 
            {
                prop.Name = Constants.Stereotypes.Rdbms.Column.PropertyId.TypeName;
            });
        }

        static bool IsImplicitColumnType(ColumnSchema column, ElementPersistable attribute)
        {
            return attribute.TypeReference.TypeId switch
            {
                Constants.TypeDefinitions.CommonTypes.String => true, // Column types for strings are handled by "Text Constraints" stereotype
                Constants.TypeDefinitions.CommonTypes.Byte when column.DbDataType.ToLower() == "tinyint" => true,
                Constants.TypeDefinitions.CommonTypes.Bool when column.DbDataType.ToLower() == "bit" => true,
                Constants.TypeDefinitions.CommonTypes.Binary when column.DbDataType.ToLower() == "varbinary" => true,
                Constants.TypeDefinitions.CommonTypes.Short when column.DbDataType.ToLower() == "smallint" => true,
                Constants.TypeDefinitions.CommonTypes.Long when column.DbDataType.ToLower() == "bigint" => true,
                Constants.TypeDefinitions.CommonTypes.Int when column.DbDataType.ToLower() == "int" => true,
                Constants.TypeDefinitions.CommonTypes.Decimal when column.DbDataType.ToLower() == "decimal" => true,
                Constants.TypeDefinitions.CommonTypes.Datetime when column.DbDataType.ToLower() == "datetime2" => true,
                Constants.TypeDefinitions.CommonTypes.Date when column.DbDataType.ToLower() == "date" => true,
                Constants.TypeDefinitions.CommonTypes.Guid when column.DbDataType.ToLower() == "uniqueidentifier" => true,
                Constants.TypeDefinitions.CommonTypes.DatetimeOffset when column.DbDataType.ToLower() == "datetimeoffset" => true,
                Constants.TypeDefinitions.CommonTypes.TimeSpan when column.DbDataType.ToLower() == "time" => true,
                _ => false
            };
        }

        static string GetColumnTypeString(ColumnSchema column)
        {
            return column.DbDataType.ToLower() switch
            {
                "varbinary" when column.MaxLength == -1 => "varbinary(max)",
                "varchar" when column.MaxLength == -1 => "varchar(max)",
                "nvarchar" when column.MaxLength == -1 => "nvarchar(max)",
                "varchar" or "nvarchar" or "varbinary" when column.MaxLength > 0 => 
                    $"{column.DbDataType.ToLower()}({column.MaxLength})",
                _ => column.DbDataType.ToLower()
            };
        }
    }

    public static void ApplyTextConstraint(ColumnSchema column, ElementPersistable attribute)
    {
        var dataType = column.LanguageDataType.ToLower();
        if (dataType != "varchar" &&
            dataType != "nvarchar" &&
            dataType != "text" &&
            dataType != "ntext")
        {
            return;
        }

        if (column.MaxLength == 0)
        {
            return;
        }

        var stereotype = attribute.GetOrCreateStereotype(Constants.Stereotypes.Rdbms.TextConstraints.DefinitionId, ster => InitTextConstraintStereotype(ster, column));
        if (column.MaxLength != -1)
        {
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.TextConstraints.PropertyId.MaxLength).Value = column.MaxLength.ToString();
        }

        return;

        static void InitTextConstraintStereotype(StereotypePersistable stereotype, ColumnSchema column)
        {
            stereotype.Name = Constants.Stereotypes.Rdbms.TextConstraints.Name;
            stereotype.DefinitionPackageName = Constants.Packages.Rdbms.DefinitionPackageName;
            stereotype.DefinitionPackageId = Constants.Packages.Rdbms.DefinitionPackageId;
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.TextConstraints.PropertyId.SqlDataType, prop =>
            {
                string value = column.DbDataType.ToUpper();
                if (value.EndsWith("MAX")) { value = value.Substring(0, value.Length - 3); }                              
                prop.Name = Constants.Stereotypes.Rdbms.TextConstraints.PropertyId.SqlDataTypeName;
                prop.Value = value;
            });
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.TextConstraints.PropertyId.MaxLength, prop =>
            {
                prop.Name = Constants.Stereotypes.Rdbms.TextConstraints.PropertyId.MaxLengthName;
                prop.Value = null;
            });
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.TextConstraints.PropertyId.IsUnicode, prop =>
            {
                prop.Name = Constants.Stereotypes.Rdbms.TextConstraints.PropertyId.IsUnicodeName;
                var dataType = column.DbDataType.ToLower();
                prop.Value = dataType == "nvarchar" || dataType == "ntext" ? "true" : "false";
            });
        }
    }

    public static void ApplyDecimalConstraint(ColumnSchema column, ElementPersistable attribute)
    {
        if (column.LanguageDataType.ToLower() != "decimal")
        {
            return;
        }

        var stereotype = attribute.GetOrCreateStereotype(Constants.Stereotypes.Rdbms.DecimalConstraints.DefinitionId, InitDecimalConstraintStereotype);
        if (column.NumericPrecision.HasValue)
        {
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.DecimalConstraints.PropertyId.Precision).Value = column.NumericPrecision.ToString();
        }
        if (column.NumericScale.HasValue)
        {
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.DecimalConstraints.PropertyId.Scale).Value = column.NumericScale.ToString();
        }
        return;

        static void InitDecimalConstraintStereotype(StereotypePersistable stereotype)
        {
            stereotype.Name = Constants.Stereotypes.Rdbms.DecimalConstraints.Name;
            stereotype.DefinitionPackageId = Constants.Packages.Rdbms.DefinitionPackageId;
            stereotype.DefinitionPackageName = Constants.Packages.Rdbms.DefinitionPackageName;
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.DecimalConstraints.PropertyId.Precision, prop => prop.Name = Constants.Stereotypes.Rdbms.DecimalConstraints.PropertyId.PrecisionName);
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.DecimalConstraints.PropertyId.Scale, prop => prop.Name = Constants.Stereotypes.Rdbms.DecimalConstraints.PropertyId.ScaleName);
        }
    }

    public static void ApplyDefaultConstraint(ColumnSchema column, ElementPersistable attribute)
    {
        if (column.DefaultConstraint == null)
        {
            return;
        }

        var stereotype = attribute.GetOrCreateStereotype(Constants.Stereotypes.Rdbms.DefaultConstraint.DefinitionId, InitDefaultConstraintStereotype);
        stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.DefaultConstraint.PropertyId.Value).Value = $@"""{column.DefaultConstraint.Text}""";
        return;

        static void InitDefaultConstraintStereotype(StereotypePersistable stereotype)
        {
            stereotype.Name = Constants.Stereotypes.Rdbms.DefaultConstraint.Name;
            stereotype.DefinitionPackageId = Constants.Packages.Rdbms.DefinitionPackageId;
            stereotype.DefinitionPackageName = Constants.Packages.Rdbms.DefinitionPackageName;
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.DefaultConstraint.PropertyId.Value, prop => prop.Name = Constants.Stereotypes.Rdbms.DefaultConstraint.PropertyId.ValueName);
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.DefaultConstraint.PropertyId.TreatAsSqlExpression, prop =>
            {
                prop.Name = Constants.Stereotypes.Rdbms.DefaultConstraint.PropertyId.TreatAsSqlExpressionName;
                prop.Value = "true";
            });
        }
    }

    public static void ApplyComputedValue(ColumnSchema column, ElementPersistable attribute)
    {
        if (column.ComputedColumn == null)
        {
            return;
        }

        var stereotype = attribute.GetOrCreateStereotype(Constants.Stereotypes.Rdbms.ComputedValue.DefinitionId, InitComputedValueStereotype);
        stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.ComputedValue.PropertyId.Sql).Value = column.ComputedColumn.Expression;
        stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.ComputedValue.PropertyId.Stored).Value = column.ComputedColumn.IsPersisted ? "true" : "false";
        return;

        static void InitComputedValueStereotype(StereotypePersistable stereotype)
        {
            stereotype.Name = Constants.Stereotypes.Rdbms.ComputedValue.Name;
            stereotype.DefinitionPackageId = Constants.Packages.Rdbms.DefinitionPackageId;
            stereotype.DefinitionPackageName = Constants.Packages.Rdbms.DefinitionPackageName;
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.ComputedValue.PropertyId.Sql, prop => prop.Name = Constants.Stereotypes.Rdbms.ComputedValue.PropertyId.SqlName);
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.ComputedValue.PropertyId.Stored, prop =>
            {
                prop.Name = Constants.Stereotypes.Rdbms.ComputedValue.PropertyId.StoredName;
                prop.Value = "false";
            });
        }
    }

    public static void ApplyForeignKey(ColumnSchema column, ElementPersistable attribute, string? associationTargetEndId = null)
    {
        var stereotype = attribute.GetOrCreateStereotype(Constants.Stereotypes.Rdbms.ForeignKey.DefinitionId, InitForeignKeyStereotype);
        
        // Link to association target end if provided
        if (!string.IsNullOrEmpty(associationTargetEndId))
        {
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.ForeignKey.PropertyId.Association).Value = associationTargetEndId;
        }

        return;

        static void InitForeignKeyStereotype(StereotypePersistable stereotype)
        {
            stereotype.Name = Constants.Stereotypes.Rdbms.ForeignKey.Name;
            stereotype.DefinitionPackageId = Constants.Packages.Rdbms.DefinitionPackageId;
            stereotype.DefinitionPackageName = Constants.Packages.Rdbms.DefinitionPackageName;
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.ForeignKey.PropertyId.Association, prop =>
            {
                prop.Name = Constants.Stereotypes.Rdbms.ForeignKey.PropertyId.AssociationName;
            });
        }
    }

    public static void ApplyStoredProcedureElementSettings(StoredProcedureSchema sqlStoredProc, ElementPersistable elementStoredProc)
    {
        var stereotype = elementStoredProc.GetOrCreateStereotype(Constants.Stereotypes.Rdbms.StoredProcedure.DefinitionId, InitStoredProcStereotype);
        if (sqlStoredProc.Name != elementStoredProc.Name)
        {
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.StoredProcedure.PropertyId.NameInSchema).Value = sqlStoredProc.Name;
        }

        for (var paramIndex = 0; paramIndex < sqlStoredProc.Parameters.Count && paramIndex < elementStoredProc.ChildElements.Count; paramIndex++)
        {
            var elementParam = elementStoredProc.ChildElements[paramIndex];
            var sqlProcParam = sqlStoredProc.Parameters[paramIndex];
            var paramStereotype = elementParam.GetOrCreateStereotype(Constants.Stereotypes.Rdbms.StoredProcedureParameter.DefinitionId, InitStoredProcParamStereotype);
            paramStereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.StoredProcedureParameter.PropertyId.IsOutputParam).Value = sqlProcParam.IsOutputParameter.ToString().ToLower();
        }

        return;

        static void InitStoredProcStereotype(StereotypePersistable stereotype)
        {
            stereotype.Name = Constants.Stereotypes.Rdbms.StoredProcedure.Name;
            stereotype.DefinitionPackageId = Constants.Packages.EntityFrameworkCoreRepository.DefinitionPackageId;
            stereotype.DefinitionPackageName = Constants.Packages.EntityFrameworkCoreRepository.DefinitionPackageName;
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.StoredProcedure.PropertyId.NameInSchema, prop => prop.Name = Constants.Stereotypes.Rdbms.StoredProcedure.PropertyId.NameInSchemaName);
        }
        
        static void InitStoredProcParamStereotype(StereotypePersistable stereotype)
        {
            stereotype.Name = Constants.Stereotypes.Rdbms.StoredProcedureParameter.Name;
            stereotype.DefinitionPackageId = Constants.Packages.EntityFrameworkCoreRepository.DefinitionPackageId;
            stereotype.DefinitionPackageName = Constants.Packages.EntityFrameworkCoreRepository.DefinitionPackageName;
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.StoredProcedureParameter.PropertyId.IsOutputParam, prop => prop.Name = Constants.Stereotypes.Rdbms.StoredProcedureParameter.PropertyId.IsOutputParamName);
        }
    }

    public static void ApplyStoredProcedureOperationSettings(StoredProcedureSchema sqlStoredProc, ElementPersistable elementStoredProc)
    {
        // Nothing yet
    }

    public static void ApplyIndexStereotype(ElementPersistable indexElement, IndexSchema index)
    {
        var stereotype = indexElement.GetOrCreateStereotype(Constants.Stereotypes.Rdbms.Index.Settings.DefinitionId, InitIndexStereotype);
        stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.Index.Settings.PropertyId.UseDefaultName).Value = "false";
        stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.Index.Settings.PropertyId.Unique).Value = index.IsUnique.ToString().ToLower();
        stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.Index.Settings.PropertyId.Filter).Value = "Default";
        return;

        static void InitIndexStereotype(StereotypePersistable stereotype)
        {
            stereotype.Name = Constants.Stereotypes.Rdbms.Index.Settings.Name;
            stereotype.DefinitionPackageId = Constants.Packages.Rdbms.DefinitionPackageId;
            stereotype.DefinitionPackageName = Constants.Packages.Rdbms.DefinitionPackageName;
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.Index.Settings.PropertyId.UseDefaultName, prop => prop.Name = Constants.Stereotypes.Rdbms.Index.Settings.PropertyId.UseDefaultNameName);
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.Index.Settings.PropertyId.Unique, prop => prop.Name = Constants.Stereotypes.Rdbms.Index.Settings.PropertyId.UniqueName);
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.Index.Settings.PropertyId.Filter, prop => prop.Name = Constants.Stereotypes.Rdbms.Index.Settings.PropertyId.FilterName);
        }
    }

    public static void ApplyIndexColumnStereotype(ElementPersistable columnIndex, IndexColumnSchema indexColumn)
    {
        // Should follow same GetOrCreateStereotype pattern here:
        var stereotype = columnIndex.GetOrCreateStereotype(Constants.Stereotypes.Rdbms.Index.IndexColumn.Settings.DefinitionId, InitIndexColumnStereotype);
        stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.Index.IndexColumn.Settings.PropertyId.Type).Value = indexColumn.IsIncluded ? "Included" : "Key";
        stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.Index.IndexColumn.Settings.PropertyId.SortDirection).Value = indexColumn.IsDescending ? "Descending" : "Ascending";
        return;

        static void InitIndexColumnStereotype(StereotypePersistable stereotype)
        {
            stereotype.Name = Constants.Stereotypes.Rdbms.Index.IndexColumn.Settings.Name;
            stereotype.DefinitionPackageId = Constants.Packages.Rdbms.DefinitionPackageId;
            stereotype.DefinitionPackageName = Constants.Packages.Rdbms.DefinitionPackageName;
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.Index.IndexColumn.Settings.PropertyId.Type, prop => prop.Name = Constants.Stereotypes.Rdbms.Index.IndexColumn.Settings.PropertyId.TypeName);
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.Index.IndexColumn.Settings.PropertyId.SortDirection, prop => prop.Name = Constants.Stereotypes.Rdbms.Index.IndexColumn.Settings.PropertyId.SortDirectionName);
        }
    }
    
    public static void AddSchemaStereotype(ElementPersistable folder, string schemaName)
    {
        folder.GetOrCreateStereotype(Constants.Stereotypes.Rdbms.Schema.DefinitionId, stereotype =>
        {
            stereotype.Name = Constants.Stereotypes.Rdbms.Schema.Name;
            stereotype.DefinitionPackageId = Constants.Packages.Rdbms.DefinitionPackageId;
            stereotype.DefinitionPackageName = Constants.Packages.Rdbms.DefinitionPackageName;
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.Schema.PropertyId.Name,
                prop =>
                {
                    prop.Name = Constants.Stereotypes.Rdbms.Schema.PropertyId.NameName;
                    prop.Value = schemaName;
                });
        });
    }
}
