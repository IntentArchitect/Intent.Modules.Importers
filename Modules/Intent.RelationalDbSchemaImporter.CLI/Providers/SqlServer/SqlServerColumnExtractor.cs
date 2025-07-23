using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using DatabaseSchemaReader.DataSchema;
using Intent.RelationalDbSchemaImporter.CLI.Providers.Core.Services;
using Intent.RelationalDbSchemaImporter.CLI.Services;
using Intent.RelationalDbSchemaImporter.Contracts.DbSchema;

namespace Intent.RelationalDbSchemaImporter.CLI.Providers.SqlServer;

internal class SqlServerColumnExtractor : DefaultColumnExtractor
{
    public override async Task<List<ColumnSchema>> ExtractTableColumnsAsync(DatabaseTable table, ImportFilterService importFilterService, DataTypeMapperBase typeMapper,
        DbConnection connection)
    {
        var columns = new List<ColumnSchema>();

        // Get computed column persistence information using T-SQL
        var computedColumnInfo = await GetComputedColumnInfoAsync(table, connection);

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
                ComputedColumn = ExtractEnhancedComputedColumn(col, computedColumnInfo)
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

        // I found that SMO and some DB Clients actually provide you with a Length for Decimal in SQL Server
        // This is somehow calculated, and it is not important at all for what we want.
        // We only want what SQL Server tells us.
        return null;
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

        return null;
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

        return null;
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
    /// Extract computed column info with IsPersisted flag using T-SQL queries
    /// </summary>
    private static async Task<Dictionary<string, bool>> GetComputedColumnInfoAsync(DatabaseTable table, DbConnection connection)
    {
        var computedColumns = new Dictionary<string, bool>();

        const string sql =
            """
            SELECT 
                c.name AS ColumnName,
                cc.is_persisted AS IsPersisted
            FROM sys.columns c
            INNER JOIN sys.tables t ON c.object_id = t.object_id
            INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
            INNER JOIN sys.computed_columns cc ON cc.object_id = t.object_id
            WHERE s.name = @SchemaName 
              AND t.name = @TableName
              AND c.is_computed = 1
            """;

        await using var command = connection.CreateCommand();
        command.CommandText = sql;

        // Add parameters
        var schemaParam = command.CreateParameter();
        schemaParam.ParameterName = "@SchemaName";
        schemaParam.Value = table.SchemaOwner ?? "dbo";
        command.Parameters.Add(schemaParam);

        var tableParam = command.CreateParameter();
        tableParam.ParameterName = "@TableName";
        tableParam.Value = table.Name;
        command.Parameters.Add(tableParam);

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var columnName = reader.GetString(0); // ColumnName
            var isPersisted = reader.GetBoolean(1); // IsPersisted
            computedColumns[columnName] = isPersisted;
        }

        return computedColumns;
    }

    /// <summary>
    /// Extract computed column with enhanced IsPersisted information
    /// </summary>
    private static ComputedColumnSchema? ExtractEnhancedComputedColumn(DatabaseColumn column, Dictionary<string, bool> computedColumnInfo)
    {
        if (column.ComputedDefinition is null)
        {
            return null;
        }

        var isPersisted = computedColumnInfo.GetValueOrDefault(column.Name, false);

        return new ComputedColumnSchema
        {
            Expression = column.ComputedDefinition,
            IsPersisted = isPersisted
        };
    }
}