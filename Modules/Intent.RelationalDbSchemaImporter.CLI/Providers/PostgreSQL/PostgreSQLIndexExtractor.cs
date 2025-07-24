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
                i.indexname AS index_name,              -- User-defined name of the index
                i.indexdef AS index_definition,         -- Complete CREATE INDEX statement
                ix.indisunique AS is_unique,            -- Whether index enforces uniqueness
                ix.indisclustered AS is_clustered,      -- Whether this is a clustered index (rarely used in PG)
                ix.indpred IS NOT NULL AS has_filter,   -- Boolean: whether index has a WHERE clause
                CAST(ix.indpred AS text) AS filter_definition, -- The actual filter expression (if any)
                
                -- Aggregate column names in index key order
                STRING_AGG(
                    CASE 
                        WHEN a.attname IS NOT NULL THEN a.attname    -- Regular column name
                        ELSE 'expression'                            -- Expression-based index column
                    END,
                    ','
                    ORDER BY array_position(ix.indkey, a.attnum)    -- Maintain index column order
                ) AS column_names,
                
                -- Aggregate sort directions for each column
                STRING_AGG(
                    CASE 
                        -- Check indoption array bit flags: bit 0 = DESC sort
                        WHEN ix.indoption[array_position(ix.indkey, a.attnum) - 1] & 1 = 1 THEN 'DESC'
                        ELSE 'ASC'
                    END,
                    ','
                    ORDER BY array_position(ix.indkey, a.attnum)    -- Maintain same order as columns
                ) AS sort_orders,
                
                -- Total number of key columns in the index
                array_length(ix.indkey, 1) AS key_column_count
            
            FROM pg_indexes i
                -- Join to get table class information
                INNER JOIN pg_class c ON c.relname = i.tablename
                
                -- Join to get schema information and ensure table is in correct schema
                INNER JOIN pg_namespace n ON n.nspname = i.schemaname 
                    AND n.oid = c.relnamespace
                
                -- Join to get detailed index metadata from pg_index catalog
                INNER JOIN pg_index ix ON ix.indexrelid = (
                    -- Subquery to find the index's OID by name within the schema
                    SELECT oid FROM pg_class 
                    WHERE relname = i.indexname 
                    AND relnamespace = n.oid
                )
                
                -- Left join to get column details for each index key column
                LEFT JOIN pg_attribute a ON a.attrelid = c.oid 
                    -- Match columns that are part of the index key
                    AND a.attnum = ANY(ix.indkey[0:array_length(ix.indkey,1)-1])  -- Column is in index key array
                    AND a.attnum > 0                                              -- Exclude system columns (negative attnum)
                
                -- Left join to identify constraint-based indexes
                LEFT JOIN pg_constraint con ON con.conindid = ix.indexrelid
            
            WHERE 
                -- Filter to specific schema and table
                i.schemaname = @schemaName
                AND i.tablename = @tableName
                
                -- Exclude primary key indexes (handled separately)
                AND ix.indisprimary = FALSE
                
                -- MAIN FILTER: Exclude indexes created for constraints
                -- contype: 'p'=primary key, 'f'=foreign key, 'u'=unique, 'c'=check
                AND (con.contype IS NULL OR con.contype NOT IN ('p', 'f'))
            
            -- Group by all non-aggregated columns to enable STRING_AGG functions
            GROUP BY 
                i.indexname, 
                i.indexdef, 
                ix.indisunique, 
                ix.indisclustered, 
                ix.indpred, 
                ix.indkey,          -- Needed for sort order calculations
                ix.indoption        -- Needed for ASC/DESC determination
            
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