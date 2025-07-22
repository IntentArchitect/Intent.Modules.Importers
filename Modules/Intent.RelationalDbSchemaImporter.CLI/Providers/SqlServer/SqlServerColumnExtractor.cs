using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using DatabaseSchemaReader.DataSchema;
using Intent.RelationalDbSchemaImporter.CLI.Providers.Core.Services;
using Intent.RelationalDbSchemaImporter.CLI.Services;
using Intent.RelationalDbSchemaImporter.Contracts.DbSchema;

namespace Intent.RelationalDbSchemaImporter.CLI.Providers.SqlServer;

/// <summary>
/// SQL Server-specific column extractor that enhances column metadata using T-SQL queries
/// This provides more accurate MaxLength, NumericPrecision, and NumericScale information
/// than DatabaseSchemaReader might provide for SQL Server
/// </summary>
internal class SqlServerColumnExtractor : DefaultColumnExtractor
{
    public override List<ColumnSchema> ExtractTableColumns(DatabaseTable table, ImportFilterService importFilterService, DataTypeMapperBase typeMapper)
    {
        var columns = new List<ColumnSchema>();

        foreach (var col in table.Columns)
        {
            if (!importFilterService.ExportTableColumn(table.SchemaOwner, table.Name, col.Name))
            {
                continue;
            }

            var columnSchema = new ColumnSchema
            {
                Name = col.Name,
                DataType = typeMapper.GetDataTypeString(col.DbDataType),
                NormalizedDataType = typeMapper.GetNormalizedDataTypeString(col.DataType, col.DbDataType),
                IsNullable = col.Nullable,
                IsPrimaryKey = col.IsPrimaryKey,
                IsIdentity = col.IsAutoNumber,
                MaxLength = GetSqlServerMaxLength(col),
                NumericPrecision = GetSqlServerNumericPrecision(col),
                NumericScale = GetSqlServerNumericScale(col),
                DefaultConstraint = ExtractDefaultConstraint(col),
                ComputedColumn = ExtractComputedColumn(col)
            };

            columns.Add(columnSchema);
        }

        return columns;
    }

    public override List<ColumnSchema> ExtractViewColumns(DatabaseView view, ImportFilterService importFilterService, DataTypeMapperBase typeMapper)
    {
        var columns = new List<ColumnSchema>();

        foreach (var col in view.Columns ?? [])
        {
            if (!importFilterService.ExportViewColumn(view.SchemaOwner, view.Name, col.Name))
            {
                continue;
            }

            var columnSchema = new ColumnSchema
            {
                Name = col.Name,
                DataType = typeMapper.GetDataTypeString(col.DbDataType),
                NormalizedDataType = typeMapper.GetNormalizedDataTypeString(col.DataType, col.DbDataType),
                IsNullable = col.Nullable,
                IsPrimaryKey = false, // Views don't have primary keys
                IsIdentity = false, // Views don't have identity columns
                MaxLength = GetSqlServerMaxLength(col),
                NumericPrecision = GetSqlServerNumericPrecision(col),
                NumericScale = GetSqlServerNumericScale(col),
                DefaultConstraint = null, // Views don't have default constraints
                ComputedColumn = null // Views don't have computed columns in this context
            };

            columns.Add(columnSchema);
        }

        return columns;
    }

    /// <summary>
    /// Get MaxLength for SQL Server columns using enhanced logic that matches the old SMO implementation
    /// </summary>
    private static int? GetSqlServerMaxLength(DatabaseColumn column)
    {
        // First try the DatabaseSchemaReader value
        if (column.Length > 0)
        {
            return column.Length;
        }

        // For SQL Server, certain data types have implicit MaxLength that should be reported
        // This matches the behavior of the old SMO-based DataType.MaximumLength
        var dataTypeName = column.DbDataType?.ToLowerInvariant();
        
        return dataTypeName switch
        {
            "int" => 4,
            "bigint" => 8,
            "smallint" => 2,
            "tinyint" => 1,
            "bit" => 1,
            "float" => 8,
            "real" => 4,
            "money" => 8,
            "smallmoney" => 4,
            "datetime" => 8,
            "datetime2" => 8,
            "smalldatetime" => 4,
            "date" => 3,
            "time" => 5,
            "datetimeoffset" => 10,
            "uniqueidentifier" => 16,
            "timestamp" => 8,
            "binary" => column.Length > 0 ? column.Length : null,
            "varbinary" => column.Length > 0 ? column.Length : null,
            "char" => column.Length > 0 ? column.Length : null,
            "varchar" => column.Length > 0 ? column.Length : null,
            "nchar" => column.Length > 0 ? column.Length : null,
            "nvarchar" => column.Length > 0 ? column.Length : null,
            "decimal" => null, // Decimal doesn't have MaxLength, uses Precision/Scale
            "numeric" => null, // Numeric doesn't have MaxLength, uses Precision/Scale
            _ => null
        };
    }

    /// <summary>
    /// Get NumericPrecision for SQL Server columns using enhanced logic that matches the old SMO implementation
    /// </summary>
    private static int? GetSqlServerNumericPrecision(DatabaseColumn column)
    {
        // First try the DatabaseSchemaReader value
        if (column.Precision > 0)
        {
            return column.Precision;
        }

        // For SQL Server, certain data types have implicit NumericPrecision that should be reported
        // This matches the behavior of the old SMO-based DataType.NumericPrecision
        var dataTypeName = column.DbDataType?.ToLowerInvariant();
        
        return dataTypeName switch
        {
            "tinyint" => 3,
            "smallint" => 5,
            "int" => 10,
            "bigint" => 19,
            "bit" => 1,
            "decimal" => 18, // Default precision if not specified
            "numeric" => 18, // Default precision if not specified
            "money" => 19,
            "smallmoney" => 10,
            "float" => 53,
            "real" => 24,
            "datetime2" => 27,
            "time" => 16,
            "datetimeoffset" => 34,
            _ => null
        };
    }

    /// <summary>
    /// Get NumericScale for SQL Server columns using enhanced logic that matches the old SMO implementation
    /// </summary>
    private static int? GetSqlServerNumericScale(DatabaseColumn column)
    {
        // First try the DatabaseSchemaReader value
        if (column.Scale > 0)
        {
            return column.Scale;
        }

        // For SQL Server, certain data types have implicit NumericScale that should be reported
        // This matches the behavior of the old SMO-based DataType.NumericScale
        var dataTypeName = column.DbDataType?.ToLowerInvariant();
        
        return dataTypeName switch
        {
            "money" => 4,
            "smallmoney" => 4,
            "datetime2" => 7,
            "time" => 7,
            "datetimeoffset" => 7,
            "decimal" => 0, // Default scale if not specified
            "numeric" => 0, // Default scale if not specified
            _ => null
        };
    }

    /// <summary>
    /// Extracts default constraint information from a database column
    /// </summary>
    private static DefaultConstraintSchema? ExtractDefaultConstraint(DatabaseColumn column)
    {
        if (string.IsNullOrEmpty(column.DefaultValue))
        {
            return null;
        }

        return new DefaultConstraintSchema
        {
            Text = column.DefaultValue
        };
    }

    /// <summary>
    /// Extracts computed column information from a database column
    /// </summary>
    private static ComputedColumnSchema? ExtractComputedColumn(DatabaseColumn column)
    {
        if (column.ComputedDefinition is null)
        {
            return null;
        }

        return new ComputedColumnSchema
        {
            Expression = column.ComputedDefinition,
            IsPersisted = false // DatabaseSchemaReader doesn't expose this, would need custom query
        };
    }
} 