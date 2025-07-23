using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Intent.RelationalDbSchemaImporter.CLI.Providers.Core;

namespace Intent.RelationalDbSchemaImporter.CLI.Providers.SqlServer;

internal class SqlServerDependencyResolver : IDependencyResolver
{
    private readonly DbConnection _connection;

    public SqlServerDependencyResolver(DbConnection connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<string>> GetDependentTablesAsync(IEnumerable<string> tableNames)
    {
        var dependentTables = new HashSet<string>();
        var processedTables = new HashSet<string>();

        // Use a queue to process dependencies recursively
        var tablesToProcess = new Queue<string>();
        
        // Add initial tables to process
        foreach (var tableName in tableNames)
        {
            tablesToProcess.Enqueue(tableName);
        }

        while (tablesToProcess.Count > 0)
        {
            var currentTable = tablesToProcess.Dequeue();
            
            // Skip if already processed
            if (processedTables.Contains(currentTable))
                continue;
                
            processedTables.Add(currentTable);

            // Get direct dependencies of current table
            var dependencies = await GetDirectTableDependenciesAsync(currentTable);
            
            foreach (var dependency in dependencies)
            {
                // Add to result set
                dependentTables.Add(dependency);
                
                // Queue for further processing if not already processed
                if (!processedTables.Contains(dependency))
                {
                    tablesToProcess.Enqueue(dependency);
                }
            }
        }

        return dependentTables;
    }

    private async Task<IEnumerable<string>> GetDirectTableDependenciesAsync(string tableName)
    {
        var parts = tableName.Split('.');
        var schema = parts.Length > 1 ? parts[0] : "dbo";
        var table = parts.Length > 1 ? parts[1] : parts[0];

        // Updated query: Find tables that THIS table references (depends on)
        const string sql = 
            """
            SELECT DISTINCT 
                SCHEMA_NAME(rt.schema_id) AS ReferencedSchema,
                rt.name AS ReferencedTable
            FROM sys.foreign_keys fk
            INNER JOIN sys.tables t ON fk.parent_object_id = t.object_id
            INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
            INNER JOIN sys.tables rt ON fk.referenced_object_id = rt.object_id
            WHERE t.name = @TableName 
              AND s.name = @SchemaName
            """;

        var dependentTables = new List<string>();

        try
        {
            await using var command = _connection.CreateCommand();
            command.CommandText = sql;

            // Add parameters
            var tableParam = command.CreateParameter();
            tableParam.ParameterName = "@TableName";
            tableParam.Value = table;
            command.Parameters.Add(tableParam);

            var schemaParam = command.CreateParameter();
            schemaParam.ParameterName = "@SchemaName";
            schemaParam.Value = schema;
            command.Parameters.Add(schemaParam);

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var referencedSchema = reader["ReferencedSchema"]?.ToString() ?? "dbo";
                var referencedTable = reader["ReferencedTable"]?.ToString() ?? "";
                
                if (!string.IsNullOrEmpty(referencedTable))
                {
                    dependentTables.Add($"{referencedSchema}.{referencedTable}");
                }
            }
        }
        catch (Exception ex)
        {
            ConsoleOutput.WarnOutput($"Failed to get SQL Server table dependencies for table {schema}.{table}: {ex.Message}");
        }

        return dependentTables;
    }
} 