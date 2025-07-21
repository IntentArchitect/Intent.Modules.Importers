using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Linq;
using Intent.RelationalDbSchemaImporter.CLI.Providers;
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
            (jsonPayload, response) =>
            {
                var request = DeserializeRequest<ImportSchemaRequest>(jsonPayload, response);
                if (request == null)
                {
                    return response;
                }

                // var config = CreateImportConfiguration(request, response);
                // if (config == null || !ValidateImportConfiguration(config, response))
                // {
                //     return response;
                // }
                //
                // var result = PerformSchemaImport(config, response);
                // if (result != null)
                // {
                //     response.SetResult(result);
                // }

                return response;
            });
    }

    public static Command CreateListStoredProceduresCommand()
    {
        return CreateStandardCommand<StoredProceduresListResult>(
            "list-stored-procedures",
            "Returns a list of stored procedures in the database",
            (jsonPayload, response) =>
            {
                var request = DeserializeRequest<ConnectionTestRequest>(jsonPayload, response);
                if (request == null || !ValidateConnectionString(request.ConnectionString, response))
                {
                    return response;
                }

                var dbConnection = CreateDatabaseConnection(request.ConnectionString, response);
                if (dbConnection == null) return response;

                using var connection = dbConnection.Value.connection;
                var db = dbConnection.Value.database;

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
            (jsonPayload, response) =>
            {
                var request = DeserializeRequest<ConnectionTestRequest>(jsonPayload, response);
                if (request == null || !ValidateConnectionString(request.ConnectionString, response))
                {
                    return response;
                }

                try
                {
                    var dbConnection = CreateDatabaseConnection(request.ConnectionString, response);
                    if (dbConnection == null)
                    {
                        return response;
                    }

                    using var connection = dbConnection.Value.connection;
                    var db = dbConnection.Value.database;

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
            (jsonPayload, response) =>
            {
                var request = DeserializeRequest<ConnectionTestRequest>(jsonPayload, response);
                if (request == null || !ValidateConnectionString(request.ConnectionString, response))
                {
                    return response;
                }

                var dbConnection = CreateDatabaseConnection(request.ConnectionString, response);
                if (dbConnection == null)
                {
                    return response;
                }

                using var connection = dbConnection.Value.connection;
                var db = dbConnection.Value.database;
        
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

    // private static ImportConfiguration? CreateImportConfiguration(ImportSchemaRequest request, StandardResponse response)
    // {
    //     var config = new ImportConfiguration
    //     {
    //         ConnectionString = request.ConnectionString,
    //         ImportFilterFilePath = request.ImportFilterFilePath,
    //         EntityNameConvention = request.EntityNameConvention,
    //         TableStereotype = request.TableStereotype,
    //         TypesToExport = request.TypesToExport,
    //         StoredProcNames = request.StoredProcNames
    //     };
    //
    //     if (!ValidateConnectionString(config.ConnectionString, response)) return null;
    //
    //     if (string.IsNullOrWhiteSpace(config.PackageFileName))
    //     {
    //         response.AddError("PackageFileName is required");
    //         return null;
    //     }
    //
    //     return config;
    // }
    //
    // private static bool ValidateImportConfiguration(ImportConfiguration config, StandardResponse response)
    // {
    //     if (!string.IsNullOrWhiteSpace(config.ImportFilterFilePath) &&
    //         !Path.IsPathRooted(config.ImportFilterFilePath))
    //     {
    //         config.ImportFilterFilePath = Path.Combine(Path.GetDirectoryName(config.PackageFileName)!, config.ImportFilterFilePath);
    //     }
    //     
    //     if (!config.ValidateFilterFile())
    //     {
    //         response.AddError("Import filter file validation failed");
    //         return false;
    //     }
    //
    //     return true;
    // }

    // private static ImportSchemaResult? PerformSchemaImport(ImportConfiguration config, StandardResponse response)
    // {
    //     try
    //     {
    //         // Use the new provider-based architecture
    //         var factory = new DatabaseProviderFactory();
    //         var databaseType = config.DatabaseType;
    //         
    //         // Auto-detect database type if not specified
    //         if (databaseType == DatabaseType.Auto)
    //         {
    //             databaseType = factory.DetectDatabaseType(config.ConnectionString);
    //             
    //             if (databaseType == DatabaseType.Auto)
    //             {
    //                 response.AddError("Could not auto-detect database type from connection string. Please specify the database type explicitly.");
    //                 return null;
    //             }
    //         }
    //
    //         var provider = factory.CreateProvider(databaseType);
    //         var databaseSchema = provider.ExtractSchemaAsync(config.ConnectionString, config).GetAwaiter().GetResult();
    //         
    //         return new ImportSchemaResult
    //         {
    //             SchemaData = databaseSchema
    //         };
    //     }
    //     catch (Exception ex)
    //     {
    //         response.AddError($"Schema extraction failed: {ex.Message}");
    //         return null;
    //     }
    // }
    
    private static (SqlConnection connection, Server server, Database database)? CreateDatabaseConnection(
        string connectionString, StandardResponse response)
    {
        try
        {
            var connection = new SqlConnection(connectionString);
            connection.Open();
            var server = new Server(new ServerConnection(connection));
            var database = server.Databases[connection.Database];
            return (connection, server, database);
        }
        catch (Exception ex)
        {
            response.AddError($"Database connection error: {ex.Message}");
            return null;
        }
    }
}
