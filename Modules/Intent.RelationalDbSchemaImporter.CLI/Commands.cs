using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intent.RelationalDbSchemaImporter.CLI.Providers;
using Intent.RelationalDbSchemaImporter.CLI.Services;
using Intent.RelationalDbSchemaImporter.Contracts.Commands;
using Intent.RelationalDbSchemaImporter.Contracts.Enums;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace Intent.RelationalDbSchemaImporter.CLI;

internal static partial class Commands
{
    public static Command CreateImportSchemaCommand()
    {
        return CreateStandardCommand<ImportSchemaResult>(
            "import-schema",
            "Imports database schema into Intent package",
            async (jsonPayload, response, cancellationToken) =>
            {
                var request = DeserializeRequest<ImportSchemaRequest>(jsonPayload, response);
                if (request == null)
                {
                    return response;
                }

                if (!ValidateConnectionString(request.ConnectionString, response))
                {
                    return response;
                }
                
                var importFilterService = new ImportFilterService(request);
                if (ValidateImportFilterFile(importFilterService, response))
                {
                    return response;
                }
                
                // Use the new provider-based architecture
                var factory = new DatabaseProviderFactory();
                var databaseType = request.DatabaseType;
                
                // Auto-detect database type if not specified
                if (databaseType == DatabaseType.Auto)
                {
                    databaseType = factory.DetectDatabaseType(request.ConnectionString);
                    
                    if (databaseType == DatabaseType.Auto)
                    {
                        response.AddError("Could not auto-detect database type from connection string. Please specify the database type explicitly.");
                        return null;
                    }
                }
                
                var provider = factory.CreateProvider(databaseType);
                var databaseSchema = await provider.ExtractSchemaAsync(request.ConnectionString, importFilterService);

                response.SetResult(new ImportSchemaResult
                {
                    SchemaData = databaseSchema
                });

                return response;
            });
    }

    public static Command CreateListStoredProceduresCommand()
    {
        return CreateStandardCommand<StoredProceduresListResult>(
            "list-stored-procedures",
            "Returns a list of stored procedures in the database",
            async (jsonPayload, response, cancellationToken) =>
            {
                var request = DeserializeRequest<ConnectionTestRequest>(jsonPayload, response);
                if (request == null || !ValidateConnectionString(request.ConnectionString, response))
                {
                    return response;
                }

                var dbConnection = await CreateDatabaseConnection(request.ConnectionString, response, cancellationToken);
                if (dbConnection == null) return response;

                using var connection = dbConnection.Connection;
                var db = dbConnection.Database;

                var result = new StoredProceduresListResult
                {
                    StoredProcedures = db.StoredProcedures
                        .OfType<StoredProcedure>()
                        .Where(x => x.Schema != "sys")
                        .Select(sp => $"{sp.Schema}.{sp.Name}")
                        .OrderBy(name => name)
                        .ToList()
                };

                response.SetResult(result);
                return response;
            });
    }

    public static Command CreateTestConnectionCommand()
    {
        return CreateStandardCommand<ConnectionTestResult>(
            "test-connection",
            "Tests the connection to the database",
            async (jsonPayload, response, cancellationToken) =>
            {
                var request = DeserializeRequest<ConnectionTestRequest>(jsonPayload, response);
                if (request == null || !ValidateConnectionString(request.ConnectionString, response))
                {
                    return response;
                }

                try
                {
                    var dbConnection = await CreateDatabaseConnection(request.ConnectionString, response, cancellationToken);
                    if (dbConnection == null)
                    {
                        return response;
                    }

                    using var connection = dbConnection.Connection;
                    var db = dbConnection.Database;

                    db.ExecuteWithResults("SELECT 1");

                    var result = new ConnectionTestResult
                    {
                        IsSuccessful = true
                    };

                    response.SetResult(result);
                    return response;
                }
                catch (Exception ex)
                {
                    var result = new ConnectionTestResult
                    {
                        IsSuccessful = false
                    };
                    response.SetResult(result);
                    response.AddError(ex.Message);
                    return response;
                }
            });
    }

    public static Command CreateRetrieveDatabaseObjectsCommand()
    {
        return CreateStandardCommand<DatabaseObjectsResult>(
            "retrieve-database-objects",
            "Extracts database metadata (tables, views, stored procedures) as JSON",
            async (jsonPayload, response, cancellationToken) =>
            {
                var request = DeserializeRequest<ConnectionTestRequest>(jsonPayload, response);
                if (request == null || !ValidateConnectionString(request.ConnectionString, response))
                {
                    return response;
                }

                var dbConnection = await CreateDatabaseConnection(request.ConnectionString, response, cancellationToken);
                if (dbConnection == null)
                {
                    return response;
                }

                using var connection = dbConnection.Connection;
                var db = dbConnection.Database;
        
                var tables = ExtractTables(db);
                var views = ExtractViews(db);
                var storedProcedures = ExtractStoredProcedures(db);

                var result = new DatabaseObjectsResult
                {
                    Tables = tables,
                    Views = views,
                    StoredProcedures = storedProcedures
                };

                response.SetResult(result);
                return response;
        
                static List<string> ExtractTables(Database db)
                {
                    return db.Tables
                        .OfType<Table>()
                        .Where(x => x.Schema != "sys")
                        .Select(t => $"{t.Schema}.{t.Name}")
                        .OrderBy(name => name)
                        .ToList();
                }

                static List<string> ExtractViews(Database db)
                {
                    return db.Views
                        .OfType<View>()
                        .Where(x => x.Schema != "sys" && x.Schema != "INFORMATION_SCHEMA")
                        .Select(v => $"{v.Schema}.{v.Name}")
                        .OrderBy(name => name)
                        .ToList();
                }

                static List<string> ExtractStoredProcedures(Database db)
                {
                    return db.StoredProcedures
                        .OfType<StoredProcedure>()
                        .Where(x => x.Schema != "sys")
                        .Select(sp => $"{sp.Schema}.{sp.Name}")
                        .OrderBy(name => name)
                        .ToList();
                }
            });
    }
    
    private static async Task<ConnectionResult?> CreateDatabaseConnection(
        string connectionString, StandardResponse response, CancellationToken cancellationToken)
    {
        try
        {
            var connection = new SqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);
            var server = new Server(new ServerConnection(connection));
            var database = server.Databases[connection.Database];
            return new ConnectionResult(connection, server, database);
        }
        catch (Exception ex)
        {
            response.AddError($"Database connection error: {ex.Message}");
            return null;
        }
    }

    private record ConnectionResult(SqlConnection Connection, Server Server, Database Database);
}
