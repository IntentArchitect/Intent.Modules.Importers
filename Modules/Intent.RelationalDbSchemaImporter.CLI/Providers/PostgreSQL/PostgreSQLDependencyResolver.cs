using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Intent.RelationalDbSchemaImporter.CLI.Providers.Core;

namespace Intent.RelationalDbSchemaImporter.CLI.Providers.PostgreSQL;

internal class PostgreSQLDependencyResolver : IDependencyResolver
{
    private readonly DbConnection _connection;

    public PostgreSQLDependencyResolver(DbConnection connection)
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
        var schema = parts.Length > 1 ? parts[0] : "public";
        var table = parts.Length > 1 ? parts[1] : parts[0];

        // Updated query: Find tables that THIS table references (depends on)
        const string sql =
            """
            SELECT DISTINCT
                ccu.table_schema AS referenced_table_schema,
                ccu.table_name AS referenced_table_name
            FROM 
                information_schema.table_constraints AS tc 
                JOIN information_schema.key_column_usage AS kcu
                  ON tc.constraint_name = kcu.constraint_name
                  AND tc.table_schema = kcu.table_schema
                JOIN information_schema.constraint_column_usage AS ccu
                  ON ccu.constraint_name = tc.constraint_name
                  AND ccu.table_schema = tc.table_schema
            WHERE 
                tc.constraint_type = 'FOREIGN KEY' 
                AND tc.table_schema = @schema 
                AND tc.table_name = @table
            """;

        var dependentTables = new List<string>();

        await using var command = _connection.CreateCommand();
        command.CommandText = sql;

        var schemaParam = command.CreateParameter();
        schemaParam.ParameterName = "@schema";
        schemaParam.Value = schema;
        command.Parameters.Add(schemaParam);

        var tableParam = command.CreateParameter();
        tableParam.ParameterName = "@table";
        tableParam.Value = table;
        command.Parameters.Add(tableParam);

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var referencedSchema = reader["referenced_table_schema"].ToString();
            var referencedTable = reader["referenced_table_name"].ToString();
            dependentTables.Add($"{referencedSchema}.{referencedTable}");
        }

        return dependentTables;
    }
}