using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intent.RelationalDbSchemaImporter.CLI.Services;
using Intent.RelationalDbSchemaImporter.Contracts.DbSchema;
using Intent.RelationalDbSchemaImporter.Contracts.Enums;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace Intent.RelationalDbSchemaImporter.CLI.Providers.SqlServer;

/// <summary>
/// SQL Server database provider implementation
/// </summary>
internal class SqlServerProvider : IDatabaseProvider
{
    public DatabaseType SupportedType => DatabaseType.SqlServer;

    public async Task<DatabaseSchema> ExtractSchemaAsync(string connectionString, ImportFilterService importFilterService)
    {
        // For now, delegate to the existing SQL Server implementation
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        var server = new Server(new ServerConnection(connection));
        var database = server.Databases[connection.Database];

        var schemaExtractor = new DatabaseSchemaExtractor(importFilterService, database);
        return schemaExtractor.ExtractSchema();
    }

    public async Task TestConnectionAsync(string connectionString, CancellationToken cancellationToken)
    {
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        var server = new Server(new ServerConnection(connection));
        var database = server.Databases[connection.Database];
        database.ExecuteWithResults("SELECT 1");
    }

    public async Task<List<string>> GetTableNamesAsync(string connectionString)
    {
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        var server = new Server(new ServerConnection(connection));
        var database = server.Databases[connection.Database];

        return database.Tables
            .OfType<Table>()
            .Where(table => !IsSystemTable(table.Schema, table.Name))
            .Select(t => $"{t.Schema}.{t.Name}")
            .OrderBy(name => name)
            .ToList();
    }

    public async Task<List<string>> GetViewNamesAsync(string connectionString)
    {
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        var server = new Server(new ServerConnection(connection));
        var database = server.Databases[connection.Database];

        return database.Views
            .OfType<View>()
            .Where(view => !IsSystemView(view.Schema, view.Name))
            .Select(v => $"{v.Schema}.{v.Name}")
            .OrderBy(name => name)
            .ToList();
    }

    public async Task<List<string>> GetRoutineNamesAsync(string connectionString)
    {
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        var server = new Server(new ServerConnection(connection));
        var database = server.Databases[connection.Database];

        return database.StoredProcedures
            .OfType<StoredProcedure>()
            .Where(sp => !IsSystemStoredProcedure(sp.Schema, sp.Name))
            .Select(sp => $"{sp.Schema}.{sp.Name}")
            .OrderBy(name => name)
            .ToList();
    }

    private static bool IsSystemTable(string schema, string name)
    {
        var systemTables = new[] { "sysdiagrams", "__MigrationHistory", "__EFMigrationsHistory" };
        return schema == "sys" || systemTables.Contains(name, StringComparer.OrdinalIgnoreCase);
    }

    private static bool IsSystemView(string schema, string name)
    {
        return schema is "sys" or "INFORMATION_SCHEMA";
    }

    private static bool IsSystemStoredProcedure(string schema, string name)
    {
        return schema == "sys";
    }
} 