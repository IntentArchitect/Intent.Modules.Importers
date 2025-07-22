using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using DatabaseSchemaReader.DataSchema;
using Intent.RelationalDbSchemaImporter.CLI.Providers.Core.Services;
using Intent.RelationalDbSchemaImporter.Contracts.DbSchema;
using Microsoft.Data.SqlClient;

namespace Intent.RelationalDbSchemaImporter.CLI.Providers.SqlServer;

/// <summary>
/// SQL Server-specific foreign key extractor with enhanced referenced column name extraction
/// Uses T-SQL queries to get proper foreign key column mapping that DatabaseSchemaReader might miss
/// </summary>
internal class SqlServerForeignKeyExtractor : DefaultForeignKeyExtractor
{
    public override async Task<List<ForeignKeySchema>> ExtractForeignKeysAsync(DatabaseTable table, DbConnection connection)
    {
        var foreignKeys = new List<ForeignKeySchema>();

        // Always try enhanced SQL Server foreign key extraction first
        var sqlServerForeignKeys = await ExtractSqlServerForeignKeysAsync(table, connection);
        
        if (sqlServerForeignKeys.Count > 0)
        {
            foreignKeys.AddRange(sqlServerForeignKeys);
        }
        else
        {
            // Fall back to DatabaseSchemaReader but enhance with SQL Server foreign key columns
            foreach (var foreignKey in table.ForeignKeys ?? [])
            {
                // Try to get SQL Server foreign key columns, but fall back to base extraction if needed
                var foreignKeyColumns = await ExtractSqlServerForeignKeyColumnsAsync(table, foreignKey.Name ?? "", connection);
                
                // If SQL Server query failed, extract basic columns from DatabaseSchemaReader
                if (foreignKeyColumns.Count == 0)
                {
                    // For SQL Server, we need to extract referenced column names from the DatabaseSchemaReader object
                    // The foreignKey.RefersToTable should contain the referenced table, and we need to map columns
                    foreignKeyColumns = ExtractBasicForeignKeyColumns(foreignKey);
                }

                var fkSchema = new ForeignKeySchema
                {
                    Name = foreignKey.Name ?? $"FK_{table.Name}",
                    ReferencedTableSchema = foreignKey.RefersToSchema ?? "",
                    ReferencedTableName = foreignKey.RefersToTable ?? "",
                    Columns = foreignKeyColumns
                };

                foreignKeys.Add(fkSchema);
            }
        }

        return foreignKeys;
    }

    /// <summary>
    /// Extract SQL Server foreign keys using T-SQL queries for complete metadata
    /// </summary>
    private async Task<List<ForeignKeySchema>> ExtractSqlServerForeignKeysAsync(DatabaseTable table, DbConnection connection)
    {
        var foreignKeys = new List<ForeignKeySchema>();

        // T-SQL query to get foreign key metadata with referenced table info
        const string sql = """
            SELECT DISTINCT
                fk.name AS ForeignKeyName,
                SCHEMA_NAME(rt.schema_id) AS ReferencedSchema,
                rt.name AS ReferencedTable
            FROM sys.foreign_keys fk
            INNER JOIN sys.tables t ON fk.parent_object_id = t.object_id
            INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
            INNER JOIN sys.tables rt ON fk.referenced_object_id = rt.object_id
            WHERE s.name = @SchemaName 
              AND t.name = @TableName
            ORDER BY fk.name
            """;

        try
        {
            // Create a new connection to avoid "DataReader already open" conflicts
            await using var newConnection = new SqlConnection(connection.ConnectionString);
            await newConnection.OpenAsync();
            
            await using var command = newConnection.CreateCommand();
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
                var foreignKeyName = reader["ForeignKeyName"]?.ToString() ?? "";
                var referencedSchema = reader["ReferencedSchema"]?.ToString() ?? "dbo";
                var referencedTable = reader["ReferencedTable"]?.ToString() ?? "";

                var foreignKeySchema = new ForeignKeySchema
                {
                    Name = foreignKeyName,
                    ReferencedTableName = referencedTable,
                    ReferencedTableSchema = referencedSchema,
                    Columns = await ExtractSqlServerForeignKeyColumnsAsync(table, foreignKeyName, connection)
                };

                foreignKeys.Add(foreignKeySchema);
            }
        }
        catch (Exception ex)
        {
            // If custom query fails, return empty list to fall back to base implementation
            ConsoleOutput.WarnOutput($"Failed to extract SQL Server foreign keys for table {table.SchemaOwner}.{table.Name}: {ex.Message}");
        }

        return foreignKeys;
    }

    /// <summary>
    /// Extract foreign key columns with proper source and referenced column names using T-SQL
    /// This addresses the issue where DatabaseSchemaReader might not properly map column relationships
    /// </summary>
    private async Task<List<ForeignKeyColumnSchema>> ExtractSqlServerForeignKeyColumnsAsync(DatabaseTable table, string foreignKeyName, DbConnection connection)
    {
        var columns = new List<ForeignKeyColumnSchema>();

        // T-SQL query to get detailed foreign key column mapping
        const string sql = """
            SELECT 
                pc.name AS SourceColumn,
                rc.name AS ReferencedColumn
            FROM sys.foreign_keys fk
            INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
            INNER JOIN sys.columns pc ON fkc.parent_object_id = pc.object_id AND fkc.parent_column_id = pc.column_id
            INNER JOIN sys.columns rc ON fkc.referenced_object_id = rc.object_id AND fkc.referenced_column_id = rc.column_id
            INNER JOIN sys.tables t ON fk.parent_object_id = t.object_id
            INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
            WHERE s.name = @SchemaName 
              AND t.name = @TableName
              AND fk.name = @ForeignKeyName
            ORDER BY fkc.constraint_column_id
            """;

        try
        {
            // Create a new connection to avoid "DataReader already open" conflicts
            await using var newConnection = new SqlConnection(connection.ConnectionString);
            await newConnection.OpenAsync();
            
            await using var command = newConnection.CreateCommand();
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

            var foreignKeyParam = command.CreateParameter();
            foreignKeyParam.ParameterName = "@ForeignKeyName";
            foreignKeyParam.Value = foreignKeyName;
            command.Parameters.Add(foreignKeyParam);

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var sourceColumn = reader["SourceColumn"]?.ToString() ?? "";
                var referencedColumn = reader["ReferencedColumn"]?.ToString() ?? "";

                var columnSchema = new ForeignKeyColumnSchema
                {
                    Name = sourceColumn,
                    ReferencedColumnName = referencedColumn
                };

                columns.Add(columnSchema);
            }
        }
        catch (Exception ex)
        {
            // If column query fails, return empty list
            ConsoleOutput.WarnOutput($"Failed to extract SQL Server foreign key columns for {table.SchemaOwner}.{table.Name}.{foreignKeyName}: {ex.Message}");
        }

        return columns;
    }

    /// <summary>
    /// Extract basic foreign key columns from DatabaseSchemaReader when custom T-SQL query fails
    /// Attempts to infer referenced column names based on common naming conventions
    /// </summary>
    private static List<ForeignKeyColumnSchema> ExtractBasicForeignKeyColumns(DatabaseConstraint foreignKey)
    {
        var columns = new List<ForeignKeyColumnSchema>();

        // DatabaseSchemaReader provides column mappings in foreign keys
        foreach (var sourceColumn in foreignKey.Columns ?? [])
        {
            var columnName = sourceColumn ?? "";
            
            // Try to infer the referenced column name based on common patterns
            var referencedColumnName = InferReferencedColumnName(columnName, foreignKey.RefersToTable ?? "");
            
            var columnSchema = new ForeignKeyColumnSchema
            {
                Name = columnName,
                ReferencedColumnName = referencedColumnName
            };

            columns.Add(columnSchema);
        }

        return columns;
    }

    /// <summary>
    /// Infer referenced column name based on common foreign key naming conventions
    /// </summary>
    private static string InferReferencedColumnName(string sourceColumnName, string referencedTableName)
    {
        // Common patterns:
        // 1. ParentCategoryID -> CategoryID (strip table prefix, handle singular/plural)
        // 2. CategoryID -> CategoryID (same name)
        // 3. UserID -> UserID (same name) 
        // 4. OrderID -> OrderID (same name)
        
        if (string.IsNullOrEmpty(referencedTableName))
        {
            return sourceColumnName; // Fallback to same name
        }

        var sourceColumnLower = sourceColumnName.ToLowerInvariant();
        var tableNameLower = referencedTableName.ToLowerInvariant();
        
        // Handle singular/plural table name patterns
        // Try both singular and plural forms of the table name
        var tableSingular = GetSingularForm(referencedTableName);
        var tableSingularLower = tableSingular.ToLowerInvariant();
        
        // Pattern 1: Check if source column ends with singular table name + "ID"
        // Example: ParentCategoryID ends with "categoryid" -> CategoryID
        if (sourceColumnLower.EndsWith(tableSingularLower + "id"))
        {
            return tableSingular + "ID";
        }
        
        // Pattern 2: Check if source column ends with plural table name + "ID"  
        // Example: ParentCategoriesID ends with "categoriesid" -> CategoriesID
        if (sourceColumnLower.EndsWith(tableNameLower + "id"))
        {
            return referencedTableName + "ID";
        }
        
        // Pattern 3: Check if source column is exactly singular table name + "ID"
        // Example: CategoryID -> CategoryID
        if (sourceColumnLower == tableSingularLower + "id")
        {
            return sourceColumnName;
        }
        
        // Pattern 4: Check if source column is exactly plural table name + "ID"
        // Example: CategoriesID -> CategoriesID
        if (sourceColumnLower == tableNameLower + "id")
        {
            return sourceColumnName;
        }
        
        // For other patterns, assume the referenced column has the same name
        // This handles cases like UserID -> UserID, OrderID -> OrderID
        return sourceColumnName;
    }

    /// <summary>
    /// Get singular form of a table name by removing common plural suffixes
    /// </summary>
    private static string GetSingularForm(string tableName)
    {
        if (string.IsNullOrEmpty(tableName))
            return tableName;
            
        // Handle common plural patterns
        if (tableName.EndsWith("ies", StringComparison.OrdinalIgnoreCase))
        {
            // Categories -> Category
            return tableName.Substring(0, tableName.Length - 3) + "y";
        }
        
        if (tableName.EndsWith("es", StringComparison.OrdinalIgnoreCase))
        {
            // Addresses -> Address, but not for words ending in single 'e'
            if (tableName.Length > 3 && !tableName.EndsWith("ses", StringComparison.OrdinalIgnoreCase))
            {
                return tableName.Substring(0, tableName.Length - 2);
            }
        }
        
        if (tableName.EndsWith("s", StringComparison.OrdinalIgnoreCase) && tableName.Length > 1)
        {
            // Users -> User, Products -> Product
            return tableName.Substring(0, tableName.Length - 1);
        }
        
        // If no plural pattern matches, return as-is
        return tableName;
    }
} 