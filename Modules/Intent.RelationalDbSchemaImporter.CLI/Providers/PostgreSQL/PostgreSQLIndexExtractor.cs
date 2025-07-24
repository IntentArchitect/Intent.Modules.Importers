using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using DatabaseSchemaReader.DataSchema;
using Intent.RelationalDbSchemaImporter.CLI.Providers.Core.Services;
using Intent.RelationalDbSchemaImporter.CLI.Services;
using Intent.RelationalDbSchemaImporter.Contracts.DbSchema;

namespace Intent.RelationalDbSchemaImporter.CLI.Providers.PostgreSQL;

internal class PostgreSQLIndexExtractor : DefaultIndexExtractor
{
    public override async Task<List<IndexSchema>> ExtractIndexesAsync(DatabaseTable table, ImportFilterService importFilterService, DbConnection connection)
    {
        var indexes = new List<IndexSchema>();

        if (!importFilterService.ExportIndexes())
        {
            return indexes;
        }

        const string sql =
            """
            SELECT 
                i.indexname AS index_name,
                i.indexdef AS index_definition,
                ix.indisunique AS is_unique,
                ix.indisclustered AS is_clustered,
                ix.indpred IS NOT NULL AS has_filter,
                CAST(ix.indpred AS text) AS filter_definition,
                STRING_AGG(
                    CASE 
                        WHEN a.attname IS NOT NULL THEN a.attname
                        ELSE 'expression'
                    END,
                    ','
                    ORDER BY array_position(ix.indkey, a.attnum)
                ) AS column_names,
                STRING_AGG(
                    CASE 
                        WHEN ix.indoption[array_position(ix.indkey, a.attnum) - 1] & 1 = 1 THEN 'DESC'
                        ELSE 'ASC'
                    END,
                    ','
                    ORDER BY array_position(ix.indkey, a.attnum)
                ) AS sort_orders,
                array_length(ix.indkey, 1) AS key_column_count
            FROM pg_indexes i
            INNER JOIN pg_class c ON c.relname = i.tablename
            INNER JOIN pg_namespace n ON n.nspname = i.schemaname AND n.oid = c.relnamespace
            INNER JOIN pg_index ix ON ix.indexrelid = (
                SELECT oid FROM pg_class WHERE relname = i.indexname AND relnamespace = n.oid
            )
            LEFT JOIN pg_attribute a ON a.attrelid = c.oid 
                AND a.attnum = ANY(ix.indkey[0:array_length(ix.indkey,1)-1])
                AND a.attnum > 0
            LEFT JOIN pg_constraint con ON con.conindid = ix.indexrelid
            WHERE i.schemaname = @schemaName
              AND i.tablename = @tableName
              AND ix.indisprimary = FALSE  -- Exclude primary key indexes
              AND (con.contype IS NULL OR con.contype NOT IN ('p', 'f'))  -- Exclude PK and FK constraint indexes
            GROUP BY i.indexname, i.indexdef, ix.indisunique, ix.indisclustered, 
                     ix.indpred, ix.indkey, ix.indoption
            ORDER BY i.indexname;
            """;

        await using var command = connection.CreateCommand();
        command.CommandText = sql;

        var schemaParam = command.CreateParameter();
        schemaParam.ParameterName = "@schemaName";
        schemaParam.Value = table.SchemaOwner ?? "public";
        command.Parameters.Add(schemaParam);

        var tableParam = command.CreateParameter();
        tableParam.ParameterName = "@tableName";
        tableParam.Value = table.Name;
        command.Parameters.Add(tableParam);

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var indexName = reader["index_name"]?.ToString() ?? "";
            var isUnique = Convert.ToBoolean(reader["is_unique"]);
            var isClustered = Convert.ToBoolean(reader["is_clustered"]);
            var hasFilter = Convert.ToBoolean(reader["has_filter"]);
            var filterDefinition = reader["filter_definition"]?.ToString();
            var columnNames = reader["column_names"]?.ToString()?.Split(',') ?? [];
            var sortOrders = reader["sort_orders"]?.ToString()?.Split(',') ?? [];

            var indexColumns = new List<IndexColumnSchema>();

            for (int i = 0; i < columnNames.Length; i++)
            {
                if (string.IsNullOrEmpty(columnNames[i]) || columnNames[i] == "expression")
                    continue;

                var isDescending = i < sortOrders.Length && sortOrders[i] == "DESC";

                var columnSchema = new IndexColumnSchema
                {
                    Name = columnNames[i].Trim(),
                    IsDescending = isDescending,
                    IsIncluded = false // For now, we'll set this to false for compatibility
                };

                indexColumns.Add(columnSchema);
            }

            var indexSchema = new IndexSchema
            {
                Name = indexName,
                IsUnique = isUnique,
                IsClustered = isClustered,
                HasFilter = hasFilter,
                FilterDefinition = filterDefinition,
                Columns = indexColumns
            };

            indexes.Add(indexSchema);
        }

        // If we didn't get any results from our custom query, fall back to base implementation
        if (indexes.Count == 0)
        {
            return await base.ExtractIndexesAsync(table, importFilterService, connection);
        }

        return indexes;
    }
} 