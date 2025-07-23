using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using DatabaseSchemaReader.DataSchema;
using Intent.RelationalDbSchemaImporter.CLI.Services;
using Intent.RelationalDbSchemaImporter.Contracts.DbSchema;

namespace Intent.RelationalDbSchemaImporter.CLI.Providers.Core.Services;

internal abstract class ColumnExtractorBase
{
    public abstract Task<List<ColumnSchema>> ExtractTableColumnsAsync(DatabaseTable table, ImportFilterService importFilterService, DataTypeMapperBase typeMapper,
        DbConnection connection);
    public abstract List<ColumnSchema> ExtractViewColumns(DatabaseView view, ImportFilterService importFilterService, DataTypeMapperBase typeMapper);
}

internal class DefaultColumnExtractor : ColumnExtractorBase
{
    /// <summary>
    /// Extracts column schema information from a database table
    /// </summary>
    public override async Task<List<ColumnSchema>> ExtractTableColumnsAsync(DatabaseTable table, ImportFilterService importFilterService, DataTypeMapperBase typeMapper,
        DbConnection connection)
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
                MaxLength = GetMaxLength(col),
                NumericPrecision = GetNumericPrecision(col),
                NumericScale = GetNumericScale(col),
                DefaultConstraint = ExtractDefaultConstraint(col),
                ComputedColumn = ExtractComputedColumn(col)
            };

            columns.Add(columnSchema);
        }

        return columns;
    }

    /// <summary>
    /// Extracts column schema information from a database view
    /// </summary>
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
                MaxLength = GetMaxLength(col),
                NumericPrecision = GetNumericPrecision(col),
                NumericScale = GetNumericScale(col),
                DefaultConstraint = null, // Views don't have default constraints
                ComputedColumn = null // Views don't have computed columns in this context
            };

            columns.Add(columnSchema);
        }

        return columns;
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
            Expression = column.ComputedDefinition
        };
    }

    // Helper methods for DatabaseColumn (separate from DatabaseArgument)
    private static int? GetMaxLength(DatabaseColumn column)
    {
        return column.Length > 0 ? column.Length : null;
    }

    private static int? GetNumericPrecision(DatabaseColumn column)
    {
        return column.Precision > 0 ? column.Precision : null;
    }

    private static int? GetNumericScale(DatabaseColumn column)
    {
        return column.Scale > 0 ? column.Scale : null;
    }
}