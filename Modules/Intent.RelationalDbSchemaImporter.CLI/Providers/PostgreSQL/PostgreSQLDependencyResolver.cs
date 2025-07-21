using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace Intent.RelationalDbSchemaImporter.CLI.Providers.PostgreSQL;

/// <summary>
/// PostgreSQL-specific dependency resolver
/// </summary>
public class PostgreSQLDependencyResolver : IDependencyResolver
{
    private readonly DbConnection _connection;

    public PostgreSQLDependencyResolver(DbConnection connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<string>> GetDependentTablesAsync(IEnumerable<string> tableNames)
    {
        var dependentTables = new HashSet<string>();
        
        foreach (var tableName in tableNames)
        {
            var dependencies = await GetTableDependenciesAsync(tableName);
            foreach (var dependency in dependencies)
            {
                dependentTables.Add(dependency);
            }
        }

        return dependentTables;
    }

    private async Task<IEnumerable<string>> GetTableDependenciesAsync(string tableName)
    {
        var parts = tableName.Split('.');
        var schema = parts.Length > 1 ? parts[0] : "public";
        var table = parts.Length > 1 ? parts[1] : parts[0];

        const string sql = @"
            SELECT 
                tc.constraint_schema,
                tc.table_name,
                kcu.column_name,
                ccu.table_schema AS foreign_table_schema,
                ccu.table_name AS foreign_table_name,
                ccu.column_name AS foreign_column_name
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
                AND ccu.table_schema = @schema 
                AND ccu.table_name = @table";

        var dependentTables = new List<string>();

        using var command = _connection.CreateCommand();
        command.CommandText = sql;
        
        var schemaParam = command.CreateParameter();
        schemaParam.ParameterName = "@schema";
        schemaParam.Value = schema;
        command.Parameters.Add(schemaParam);

        var tableParam = command.CreateParameter();
        tableParam.ParameterName = "@table";
        tableParam.Value = table;
        command.Parameters.Add(tableParam);

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var dependentSchema = reader["constraint_schema"].ToString();
            var dependentTable = reader["table_name"].ToString();
            dependentTables.Add($"{dependentSchema}.{dependentTable}");
        }

        return dependentTables;
    }
} 