using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using DatabaseSchemaReader.DataSchema;
using Intent.RelationalDbSchemaImporter.CLI.Providers.Core.Services;
using Intent.RelationalDbSchemaImporter.CLI.Services;
using Intent.RelationalDbSchemaImporter.Contracts.DbSchema;
using Microsoft.Data.SqlClient;

namespace Intent.RelationalDbSchemaImporter.CLI.Providers.SqlServer;

internal class SqlServerIndexExtractor : DefaultIndexExtractor
{
    public override async Task<List<IndexSchema>> ExtractIndexesAsync(DatabaseTable table, ImportFilterService importFilterService, DbConnection connection)
    {
        var indexes = new List<IndexSchema>();

        if (!importFilterService.ExportIndexes())
        {
            return indexes;
        }

        var sqlServerIndexes = await ExtractSqlServerIndexesAsync(table, connection);
        if (sqlServerIndexes.Count > 0)
        {
            // Use custom query results which have proper column information
            indexes.AddRange(sqlServerIndexes);
        }

        // Apply SQL Server specific filtering (migrated from DatabaseSchemaExtractor logic)
        // Skip clustered indexes as per original DatabaseSchemaExtractor.ExtractTableIndexes logic
        return indexes.Where(index => !index.IsClustered).ToList();
    }

    private async Task<List<IndexSchema>> ExtractSqlServerIndexesAsync(DatabaseTable table, DbConnection connection)
    {
        var indexes = new List<IndexSchema>();

        // Enhanced T-SQL query to get comprehensive index metadata
        const string sql =
            """
            SELECT 
                i.name AS IndexName,
                i.is_unique AS IsUnique,
                i.type_desc AS IndexType,
                i.is_primary_key AS IsPrimaryKey,
                i.has_filter AS HasFilter,
                i.filter_definition AS FilterDefinition,
                CASE WHEN i.type = 1 THEN 1 ELSE 0 END AS IsClustered
            FROM sys.indexes i
            INNER JOIN sys.tables t ON i.object_id = t.object_id
            INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
            WHERE s.name = @SchemaName 
              AND t.name = @TableName
              AND i.index_id > 0  -- Exclude heap (index_id = 0)
              AND i.name IS NOT NULL  -- Exclude unnamed indexes
              AND i.is_primary_key = 0  -- Exclude primary key constraints (handled at column level)
            ORDER BY i.name
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

        await using (var reader = await command.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                var indexName = reader["IndexName"]?.ToString() ?? "";
                var isUnique = Convert.ToBoolean(reader["IsUnique"]);
                var isClustered = Convert.ToBoolean(reader["IsClustered"]);
                var hasFilter = Convert.ToBoolean(reader["HasFilter"]);
                var filterDefinition = reader["FilterDefinition"]?.ToString();
                filterDefinition = string.IsNullOrWhiteSpace(filterDefinition) ? null : filterDefinition;
                
                var indexSchema = new IndexSchema
                {
                    Name = indexName,
                    IsUnique = isUnique,
                    IsClustered = isClustered,
                    HasFilter = hasFilter,
                    FilterDefinition = filterDefinition,
                    // Don't set Columns here due to the need of another SQL Connection inside an existing one
                };

                indexes.Add(indexSchema);
            }
        }

        foreach (var index in indexes)
        {
            index.Columns = await ExtractSqlServerIndexColumnsAsync(table, index.Name, connection);
        }

        return indexes;
    }

    private async Task<List<IndexColumnSchema>> ExtractSqlServerIndexColumnsAsync(DatabaseTable table, string indexName, DbConnection connection)
    {
        var columns = new List<IndexColumnSchema>();

        // T-SQL query to get index column details including sort order and included columns
        const string sql =
            """
            SELECT 
                c.name AS ColumnName,
                ic.is_descending_key AS IsDescending,
                ic.is_included_column AS IsIncluded,
                ic.key_ordinal AS KeyOrdinal
            FROM sys.indexes i
            INNER JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
            INNER JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
            INNER JOIN sys.tables t ON i.object_id = t.object_id
            INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
            WHERE s.name = @SchemaName 
              AND t.name = @TableName
              AND i.name = @IndexName
            ORDER BY ic.key_ordinal, ic.index_column_id
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

        var indexParam = command.CreateParameter();
        indexParam.ParameterName = "@IndexName";
        indexParam.Value = indexName;
        command.Parameters.Add(indexParam);

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var columnName = reader["ColumnName"]?.ToString() ?? "";
            var isDescending = Convert.ToBoolean(reader["IsDescending"]);
            var isIncluded = Convert.ToBoolean(reader["IsIncluded"]);

            var columnSchema = new IndexColumnSchema
            {
                Name = columnName,
                IsDescending = isDescending,
                IsIncluded = isIncluded
            };

            columns.Add(columnSchema);
        }


        return columns;
    }
}