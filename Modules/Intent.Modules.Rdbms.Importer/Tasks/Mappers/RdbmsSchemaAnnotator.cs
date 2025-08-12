using System;
using System.Diagnostics;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Rdbms.Importer.Tasks.Helpers;
using Intent.RelationalDbSchemaImporter.Contracts.DbSchema;
using Intent.RelationalDbSchemaImporter.Contracts.Enums;

namespace Intent.Modules.Rdbms.Importer.Tasks.Mappers;

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
                TableStereotype.WhenDifferent when config.EntityNameConvention is EntityNameConvention.SingularEntity => @class.Name.Pluralize() !=
                    table.Name, // This assumes EF convention
                _ => false
            };
        }

        static void InitTableStereotype(StereotypePersistable stereotype)
        {
            stereotype.Name = Constants.Stereotypes.Rdbms.Table.Name;
            stereotype.DefinitionPackageId = Constants.Packages.Rdbms.DefinitionPackageId;
            stereotype.DefinitionPackageName = Constants.Packages.Rdbms.DefinitionPackageName;
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.Table.PropertyId.Name, prop => { prop.Name = Constants.Stereotypes.Rdbms.Table.PropertyId.NameName; });
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.Table.PropertyId.Schema, prop => { prop.Name = Constants.Stereotypes.Rdbms.Table.PropertyId.SchemaName; });
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
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.View.PropertyId.Name, prop => { prop.Name = Constants.Stereotypes.Rdbms.View.PropertyId.NameName; });
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.View.PropertyId.Schema, prop => { prop.Name = Constants.Stereotypes.Rdbms.View.PropertyId.SchemaName; });
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

        attribute.SetElementMetadata("is-managed-key", "true");
        return;

        static void InitPrimaryKeyStereotype(StereotypePersistable stereotype)
        {
            stereotype.Name = Constants.Stereotypes.Rdbms.PrimaryKey.Name;
            stereotype.DefinitionPackageId = Constants.Packages.Rdbms.DefinitionPackageId;
            stereotype.DefinitionPackageName = Constants.Packages.Rdbms.DefinitionPackageName;
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.PrimaryKey.PropertyId.DataSource,
                prop => { prop.Name = Constants.Stereotypes.Rdbms.PrimaryKey.PropertyId.DataSourceName; });
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
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.Column.PropertyId.Name, prop => { prop.Name = Constants.Stereotypes.Rdbms.Column.PropertyId.NameName; });
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.Column.PropertyId.Type, prop => { prop.Name = Constants.Stereotypes.Rdbms.Column.PropertyId.TypeName; });
        }

        static bool IsImplicitColumnType(ColumnSchema column, ElementPersistable attribute)
        {
            return attribute.TypeReference.TypeId switch
            {
                Constants.TypeDefinitions.CommonTypes.String => ShouldUseTextConstraints(column), // Only implicit if handled by Text Constraints
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
        if (!ShouldUseTextConstraints(column))
        {
            return;
        }

        var stereotype = attribute.GetOrCreateStereotype(Constants.Stereotypes.Rdbms.TextConstraints.DefinitionId, ster => InitTextConstraintStereotype(ster, column));
        if (column.MaxLength != -1)
        {
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.TextConstraints.PropertyId.MaxLength).Value = column.MaxLength?.ToString()!;
        }

        return;

        static void InitTextConstraintStereotype(StereotypePersistable stereotype, ColumnSchema column)
        {
            stereotype.Name = Constants.Stereotypes.Rdbms.TextConstraints.Name;
            stereotype.DefinitionPackageName = Constants.Packages.Rdbms.DefinitionPackageName;
            stereotype.DefinitionPackageId = Constants.Packages.Rdbms.DefinitionPackageId;
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.TextConstraints.PropertyId.SqlDataType, prop =>
            {
                var value = column.DbDataType.ToUpper();
                if (value.EndsWith("MAX"))
                {
                    value = value.Substring(0, value.Length - 3);
                }

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
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.DecimalConstraints.PropertyId.Precision).Value = column.NumericPrecision?.ToString()!;
        }

        if (column.NumericScale.HasValue)
        {
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.DecimalConstraints.PropertyId.Scale).Value = column.NumericScale?.ToString()!;
        }

        return;

        static void InitDecimalConstraintStereotype(StereotypePersistable stereotype)
        {
            stereotype.Name = Constants.Stereotypes.Rdbms.DecimalConstraints.Name;
            stereotype.DefinitionPackageId = Constants.Packages.Rdbms.DefinitionPackageId;
            stereotype.DefinitionPackageName = Constants.Packages.Rdbms.DefinitionPackageName;
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.DecimalConstraints.PropertyId.Precision,
                prop => prop.Name = Constants.Stereotypes.Rdbms.DecimalConstraints.PropertyId.PrecisionName);
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.DecimalConstraints.PropertyId.Scale,
                prop => prop.Name = Constants.Stereotypes.Rdbms.DecimalConstraints.PropertyId.ScaleName);
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
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.DefaultConstraint.PropertyId.Value,
                prop => prop.Name = Constants.Stereotypes.Rdbms.DefaultConstraint.PropertyId.ValueName);
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
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.ComputedValue.PropertyId.Sql,
                prop => prop.Name = Constants.Stereotypes.Rdbms.ComputedValue.PropertyId.SqlName);
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.ComputedValue.PropertyId.Stored, prop =>
            {
                prop.Name = Constants.Stereotypes.Rdbms.ComputedValue.PropertyId.StoredName;
                prop.Value = "false";
            });
        }
    }

    public static void ApplyForeignKey(ColumnSchema column, ElementPersistable attribute, string? associationTargetEndId)
    {
        var stereotype = attribute.GetOrCreateStereotype(Constants.Stereotypes.Rdbms.ForeignKey.DefinitionId, InitForeignKeyStereotype);

        // Link to the association target end if provided
        if (!string.IsNullOrEmpty(associationTargetEndId))
        {
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.ForeignKey.PropertyId.Association).Value = associationTargetEndId;

            attribute.SetElementMetadata("fk-original-name", attribute.Name);
            attribute.SetElementMetadata("association", associationTargetEndId);
            attribute.SetElementMetadata("is-managed-key", "true");
        }
        else
        {
            attribute.RemoveElementMetadata("fk-original-name");
            attribute.RemoveElementMetadata("association");
            attribute.RemoveElementMetadata("is-managed-key");
        }

        return;

        static void InitForeignKeyStereotype(StereotypePersistable stereotype)
        {
            stereotype.Name = Constants.Stereotypes.Rdbms.ForeignKey.Name;
            stereotype.DefinitionPackageId = Constants.Packages.Rdbms.DefinitionPackageId;
            stereotype.DefinitionPackageName = Constants.Packages.Rdbms.DefinitionPackageName;
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.ForeignKey.PropertyId.Association,
                prop => { prop.Name = Constants.Stereotypes.Rdbms.ForeignKey.PropertyId.AssociationName; });
        }
    }

    public static void ApplyStoredProcedureElementSettings(StoredProcedureSchema sqlStoredProc, ElementPersistable elementStoredProc)
    {
        var stereotype = elementStoredProc.GetOrCreateStereotype(Constants.Stereotypes.Rdbms.StoredProcedureElement.DefinitionId, InitStoredProcStereotype);
        if (sqlStoredProc.Name != elementStoredProc.Name)
        {
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.StoredProcedureElement.PropertyId.NameInSchema).Value = GetSchemaStoredProcedureName(sqlStoredProc);
        }

        for (var paramIndex = 0; paramIndex < sqlStoredProc.Parameters.Count && paramIndex < elementStoredProc.ChildElements.Count; paramIndex++)
        {
            var elementParam = elementStoredProc.ChildElements[paramIndex];
            var sqlProcParam = sqlStoredProc.Parameters[paramIndex];
            var paramStereotype = elementParam.GetOrCreateStereotype(Constants.Stereotypes.Rdbms.StoredProcedureElementParameter.DefinitionId, InitStoredProcParamStereotype);
            paramStereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.StoredProcedureElementParameter.PropertyId.IsOutputParam).Value =
                sqlProcParam.Direction == StoredProcedureParameterDirection.Out ? "true" : "false";

            switch (sqlProcParam.Direction)
            {
                case StoredProcedureParameterDirection.Out or StoredProcedureParameterDirection.Both when
                    sqlProcParam.LanguageDataType == "string":
                {
                    paramStereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.StoredProcedureElementParameter.PropertyId.SqlStringType).Value =
                        MapToSqlStringTypeValue(sqlProcParam.DbDataType)!;
                    if (sqlProcParam.MaxLength.HasValue)
                    {
                        paramStereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.StoredProcedureElementParameter.PropertyId.Size).Value =
                            sqlProcParam.MaxLength.Value.ToString();
                    }

                    break;
                }
                case StoredProcedureParameterDirection.Out or StoredProcedureParameterDirection.Both when
                    sqlProcParam.LanguageDataType == "decimal":
                {
                    paramStereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.StoredProcedureElementParameter.PropertyId.SqlStringType).Value =
                        MapToSqlStringTypeValue(sqlProcParam.DbDataType)!;
                    if (sqlProcParam.NumericPrecision.HasValue)
                    {
                        paramStereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.StoredProcedureElementParameter.PropertyId.Precision).Value =
                            sqlProcParam.NumericPrecision.Value.ToString();
                    }

                    if (sqlProcParam.NumericScale.HasValue)
                    {
                        paramStereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.StoredProcedureElementParameter.PropertyId.Scale).Value =
                            sqlProcParam.NumericScale.Value.ToString();
                    }

                    break;
                }
            }
        }

        return;

        static string GetSchemaStoredProcedureName(StoredProcedureSchema sqlStoredProc)
        {
            if (sqlStoredProc.Schema is null or "dbo")
            {
                return sqlStoredProc.Name;
            }

            return $"{sqlStoredProc.Schema}.{sqlStoredProc.Name}";
        }

        static void InitStoredProcStereotype(StereotypePersistable stereotype)
        {
            stereotype.Name = Constants.Stereotypes.Rdbms.StoredProcedureElement.Name;
            stereotype.DefinitionPackageId = Constants.Packages.EntityFrameworkCoreRepository.DefinitionPackageId;
            stereotype.DefinitionPackageName = Constants.Packages.EntityFrameworkCoreRepository.DefinitionPackageName;
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.StoredProcedureElement.PropertyId.NameInSchema,
                prop => prop.Name = Constants.Stereotypes.Rdbms.StoredProcedureElement.PropertyId.NameInSchemaName);
        }

        static void InitStoredProcParamStereotype(StereotypePersistable stereotype)
        {
            stereotype.Name = Constants.Stereotypes.Rdbms.StoredProcedureElementParameter.Name;
            stereotype.DefinitionPackageId = Constants.Packages.EntityFrameworkCoreRepository.DefinitionPackageId;
            stereotype.DefinitionPackageName = Constants.Packages.EntityFrameworkCoreRepository.DefinitionPackageName;
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.StoredProcedureElementParameter.PropertyId.IsOutputParam,
                prop => prop.Name = Constants.Stereotypes.Rdbms.StoredProcedureElementParameter.PropertyId.IsOutputParamName);
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.StoredProcedureElementParameter.PropertyId.SqlStringType,
                prop => prop.Name = Constants.Stereotypes.Rdbms.StoredProcedureElementParameter.PropertyId.SqlStringTypeName);
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.StoredProcedureElementParameter.PropertyId.Size,
                prop => prop.Name = Constants.Stereotypes.Rdbms.StoredProcedureElementParameter.PropertyId.SizeName);
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.StoredProcedureElementParameter.PropertyId.Precision,
                prop => prop.Name = Constants.Stereotypes.Rdbms.StoredProcedureElementParameter.PropertyId.PrecisionName);
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.StoredProcedureElementParameter.PropertyId.Scale,
                prop => prop.Name = Constants.Stereotypes.Rdbms.StoredProcedureElementParameter.PropertyId.ScaleName);
        }
    }

    public static void ApplyStoredProcedureOperationSettings(StoredProcedureSchema sqlStoredProc, ElementPersistable elementStoredProc)
    {
        var stereotype = elementStoredProc.GetOrCreateStereotype(Constants.Stereotypes.Rdbms.StoredProcedureOperation.DefinitionId, InitStoredProcOperationStereotype);
        if (sqlStoredProc.Name != elementStoredProc.Name)
        {
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.StoredProcedureOperation.PropertyId.NameInSchema).Value = GetSchemaStoredProcedureName(sqlStoredProc);
        }

        for (var paramIndex = 0; paramIndex < sqlStoredProc.Parameters.Count && paramIndex < elementStoredProc.ChildElements.Count; paramIndex++)
        {
            var elementParam = elementStoredProc.ChildElements[paramIndex];
            var sqlProcParam = sqlStoredProc.Parameters[paramIndex];
            var paramStereotype =
                elementParam.GetOrCreateStereotype(Constants.Stereotypes.Rdbms.StoredProcedureOperationParameter.DefinitionId, InitStoredProcOperationParamStereotype);

            paramStereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.StoredProcedureOperationParameter.PropertyId.ParameterName).Value = sqlProcParam.Name;
            paramStereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.StoredProcedureOperationParameter.PropertyId.Direction).Value = sqlProcParam.Direction switch
            {
                StoredProcedureParameterDirection.In => "In",
                StoredProcedureParameterDirection.Out => "Out",
                StoredProcedureParameterDirection.Both => "Both",
                _ => throw new ArgumentOutOfRangeException(nameof(sqlProcParam.Direction), sqlProcParam.Direction, null)
            };

            switch (sqlProcParam.Direction)
            {
                case StoredProcedureParameterDirection.Out or StoredProcedureParameterDirection.Both when
                    sqlProcParam.LanguageDataType == "string":
                {
                    paramStereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.StoredProcedureOperationParameter.PropertyId.SqlStringType).Value =
                        MapToSqlStringTypeValue(sqlProcParam.DbDataType)!;
                    if (sqlProcParam.MaxLength.HasValue)
                    {
                        paramStereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.StoredProcedureOperationParameter.PropertyId.Size).Value =
                            sqlProcParam.MaxLength.Value.ToString();
                    }

                    break;
                }
                case StoredProcedureParameterDirection.Out or StoredProcedureParameterDirection.Both when
                    sqlProcParam.LanguageDataType == "decimal":
                {
                    paramStereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.StoredProcedureOperationParameter.PropertyId.SqlStringType).Value =
                        MapToSqlStringTypeValue(sqlProcParam.DbDataType)!;
                    if (sqlProcParam.NumericPrecision.HasValue)
                    {
                        paramStereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.StoredProcedureOperationParameter.PropertyId.Precision).Value =
                            sqlProcParam.NumericPrecision.Value.ToString();
                    }

                    if (sqlProcParam.NumericScale.HasValue)
                    {
                        paramStereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.StoredProcedureOperationParameter.PropertyId.Scale).Value =
                            sqlProcParam.NumericScale.Value.ToString();
                    }

                    break;
                }
            }
        }

        return;

        static string GetSchemaStoredProcedureName(StoredProcedureSchema sqlStoredProc)
        {
            if (sqlStoredProc.Schema is null or "dbo")
            {
                return sqlStoredProc.Name;
            }

            return $"{sqlStoredProc.Schema}.{sqlStoredProc.Name}";
        }

        static void InitStoredProcOperationStereotype(StereotypePersistable stereotype)
        {
            stereotype.Name = Constants.Stereotypes.Rdbms.StoredProcedureOperation.Name;
            stereotype.DefinitionPackageId = Constants.Packages.EntityFrameworkCoreRepository.DefinitionPackageId;
            stereotype.DefinitionPackageName = Constants.Packages.EntityFrameworkCoreRepository.DefinitionPackageName;
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.StoredProcedureOperation.PropertyId.NameInSchema,
                prop => prop.Name = Constants.Stereotypes.Rdbms.StoredProcedureOperation.PropertyId.NameInSchemaName);
        }

        static void InitStoredProcOperationParamStereotype(StereotypePersistable stereotype)
        {
            stereotype.Name = Constants.Stereotypes.Rdbms.StoredProcedureOperationParameter.Name;
            stereotype.DefinitionPackageId = Constants.Packages.EntityFrameworkCoreRepository.DefinitionPackageId;
            stereotype.DefinitionPackageName = Constants.Packages.EntityFrameworkCoreRepository.DefinitionPackageName;
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.StoredProcedureOperationParameter.PropertyId.ParameterName,
                prop => prop.Name = Constants.Stereotypes.Rdbms.StoredProcedureOperationParameter.PropertyId.ParameterNameName);
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.StoredProcedureOperationParameter.PropertyId.Direction,
                prop => prop.Name = Constants.Stereotypes.Rdbms.StoredProcedureOperationParameter.PropertyId.DirectionName);
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.StoredProcedureOperationParameter.PropertyId.SqlStringType,
                prop => prop.Name = Constants.Stereotypes.Rdbms.StoredProcedureOperationParameter.PropertyId.SqlStringTypeName);
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.StoredProcedureOperationParameter.PropertyId.Size,
                prop => prop.Name = Constants.Stereotypes.Rdbms.StoredProcedureOperationParameter.PropertyId.SizeName);
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.StoredProcedureOperationParameter.PropertyId.Precision,
                prop => prop.Name = Constants.Stereotypes.Rdbms.StoredProcedureOperationParameter.PropertyId.PrecisionName);
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.StoredProcedureOperationParameter.PropertyId.Scale,
                prop => prop.Name = Constants.Stereotypes.Rdbms.StoredProcedureOperationParameter.PropertyId.ScaleName);
        }
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
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.Index.Settings.PropertyId.UseDefaultName,
                prop => prop.Name = Constants.Stereotypes.Rdbms.Index.Settings.PropertyId.UseDefaultNameName);
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.Index.Settings.PropertyId.Unique,
                prop => prop.Name = Constants.Stereotypes.Rdbms.Index.Settings.PropertyId.UniqueName);
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.Index.Settings.PropertyId.Filter,
                prop => prop.Name = Constants.Stereotypes.Rdbms.Index.Settings.PropertyId.FilterName);
        }
    }
    
    public static void ApplyIndexColumnStereotype(ElementPersistable columnIndex, IndexColumnSchema indexColumn)
    {
        // Should follow same GetOrCreateStereotype pattern here:
        var stereotype = columnIndex.GetOrCreateStereotype(Constants.Stereotypes.Rdbms.Index.IndexColumn.Settings.DefinitionId, InitIndexColumnStereotype);
        stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.Index.IndexColumn.Settings.PropertyId.Type).Value = indexColumn.IsIncluded ? "Included" : "Key";
        stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.Index.IndexColumn.Settings.PropertyId.SortDirection).Value =
            indexColumn.IsDescending ? "Descending" : "Ascending";
        return;
    
        static void InitIndexColumnStereotype(StereotypePersistable stereotype)
        {
            stereotype.Name = Constants.Stereotypes.Rdbms.Index.IndexColumn.Settings.Name;
            stereotype.DefinitionPackageId = Constants.Packages.Rdbms.DefinitionPackageId;
            stereotype.DefinitionPackageName = Constants.Packages.Rdbms.DefinitionPackageName;
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.Index.IndexColumn.Settings.PropertyId.Type,
                prop => prop.Name = Constants.Stereotypes.Rdbms.Index.IndexColumn.Settings.PropertyId.TypeName);
            stereotype.GetOrCreateProperty(Constants.Stereotypes.Rdbms.Index.IndexColumn.Settings.PropertyId.SortDirection,
                prop => prop.Name = Constants.Stereotypes.Rdbms.Index.IndexColumn.Settings.PropertyId.SortDirectionName);
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
    
    private static bool ShouldUseTextConstraints(ColumnSchema column)
    {
        var dataType = column.LanguageDataType.ToLower();
        if (dataType != "string")
        {
            return false;
        }

        if (column.MaxLength == 0)
        {
            return false;
        }

        var sqlType = column.DbDataType.ToLower();
        return sqlType is "ntext" or "nvarchar" or "text" or "varchar";
    }
    
    private static string? MapToSqlStringTypeValue(string? dbDataType)
    {
        if (dbDataType is null)
        {
            return null;
        }

        return dbDataType.ToLowerInvariant() switch
        {
            "varchar" => "VarChar",
            "nvarchar" => "NVarChar",
            "char" => "Char",
            "nchar" => "NChar",
            "text" => "Text",
            "ntext" => "NText",
            _ => null
        };
    }
}