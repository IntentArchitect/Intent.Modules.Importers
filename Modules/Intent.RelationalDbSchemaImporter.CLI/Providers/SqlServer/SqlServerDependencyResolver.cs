using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Intent.RelationalDbSchemaImporter.CLI.Providers.Core;

namespace Intent.RelationalDbSchemaImporter.CLI.Providers.SqlServer;

/// <summary>
/// SQL Server-specific dependency resolver using T-SQL queries to find table dependencies
/// </summary>
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

    /// <summary>
    /// Get tables that depend on the specified table through foreign key relationships
    /// Uses T-SQL queries to sys.foreign_keys and related system views
    /// </summary>
    private async Task<IEnumerable<string>> GetTableDependenciesAsync(string tableName)
    {
        var parts = tableName.Split('.');
        var schema = parts.Length > 1 ? parts[0] : "dbo";
        var table = parts.Length > 1 ? parts[1] : parts[0];

        // Query to find tables that reference this table via foreign keys
        const string sql = """
            SELECT DISTINCT 
                SCHEMA_NAME(t.schema_id) AS DependentSchema,
                t.name AS DependentTable
            FROM sys.foreign_keys fk
            INNER JOIN sys.tables t ON fk.parent_object_id = t.object_id
            INNER JOIN sys.tables rt ON fk.referenced_object_id = rt.object_id
            INNER JOIN sys.schemas rs ON rt.schema_id = rs.schema_id
            WHERE rt.name = @TableName 
              AND rs.name = @SchemaName
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
                var dependentSchema = reader["DependentSchema"]?.ToString() ?? "dbo";
                var dependentTable = reader["DependentTable"]?.ToString() ?? "";
                
                if (!string.IsNullOrEmpty(dependentTable))
                {
                    dependentTables.Add($"{dependentSchema}.{dependentTable}");
                }
            }
        }
        catch
        {
            // If dependency resolution fails, return empty list
            // This is not critical for the import process
        }

        return dependentTables;
    }
} 