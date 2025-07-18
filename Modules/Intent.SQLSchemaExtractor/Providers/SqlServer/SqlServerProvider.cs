using System.Data.Common;
using Microsoft.Data.SqlClient;
using Intent.RelationalDbSchemaImporter.Contracts.Schema;
using Intent.SQLSchemaExtractor.Extractors;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;
using System;
using System.Threading.Tasks;

namespace Intent.SQLSchemaExtractor.Providers.SqlServer;

/// <summary>
/// SQL Server database provider implementation
/// </summary>
public class SqlServerProvider : IDatabaseProvider
{
    public DatabaseType SupportedType => DatabaseType.SqlServer;

    public async Task<DatabaseSchema> ExtractSchemaAsync(string connectionString, ImportConfiguration config)
    {
        // For now, delegate to the existing SQL Server implementation
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        var server = new Server(new ServerConnection(connection));
        var database = server.Databases[connection.Database];

        var schemaExtractor = new DatabaseSchemaExtractor(config, database);
        return schemaExtractor.ExtractSchema();
    }

    public async Task<bool> TestConnectionAsync(string connectionString)
    {
        try
        {
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
} 