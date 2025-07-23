using System;
using System.CommandLine;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Intent.RelationalDbSchemaImporter.CLI.Providers;
using Intent.RelationalDbSchemaImporter.CLI.Providers.Core;
using Intent.RelationalDbSchemaImporter.CLI.Services;
using Intent.RelationalDbSchemaImporter.Contracts.Commands;
using Intent.RelationalDbSchemaImporter.Contracts.Enums;
using Intent.Utils;

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
                if (!importFilterService.ValidateFilterFile(out var errors))
                {
                    response.Errors.AddRange(errors);
                    return response;
                }
                
                // Use the new provider-based architecture
                var factory = new DatabaseProviderFactory();
                var databaseType = request.DatabaseType;

                var provider = factory.CreateProvider(databaseType);
                var databaseSchema = await provider.ExtractSchemaAsync(request.ConnectionString, importFilterService, cancellationToken);

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
                var request = DeserializeRequest<StoredProceduresListRequest>(jsonPayload, response);
                if (request == null || !ValidateConnectionString(request.ConnectionString, response) ||
                    !ValidateDatabaseType(request.DatabaseType, response))
                {
                    return response;
                }

                try
                {
                    var factory = new DatabaseProviderFactory();
                    var provider = factory.CreateProvider(request.DatabaseType);
                    
                    var routines = await provider.GetStoredProcedureNamesAsync(request.ConnectionString);
                    
                    var result = new StoredProceduresListResult
                    {
                        StoredProcedures = routines
                    };

                    response.SetResult(result);
                    return response;
                }
                catch (Exception ex)
                {
                    response.AddError($"Error retrieving stored procedures: {ex.Message}");
                    return response;
                }
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
                if (request == null || !ValidateConnectionString(request.ConnectionString, response) ||
                    !ValidateDatabaseType(request.DatabaseType, response))
                {
                    return response;
                }

                try
                {
                    var factory = new DatabaseProviderFactory();
                    var provider = factory.CreateProvider(request.DatabaseType);
                    
                    await provider.TestConnectionAsync(request.ConnectionString, cancellationToken);
                    
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
                var request = DeserializeRequest<DatabaseObjectsRequest>(jsonPayload, response);
                if (request == null || !ValidateConnectionString(request.ConnectionString, response) ||
                    !ValidateDatabaseType(request.DatabaseType, response))
                {
                    return response;
                }

                try
                {
                    var factory = new DatabaseProviderFactory();
                    var provider = factory.CreateProvider(request.DatabaseType);
                    
                    var tables = await provider.GetTableNamesAsync(request.ConnectionString);
                    var views = await provider.GetViewNamesAsync(request.ConnectionString);
                    var storedProcedures = await provider.GetStoredProcedureNamesAsync(request.ConnectionString);

                    var result = new DatabaseObjectsResult
                    {
                        Tables = tables,
                        Views = views,
                        StoredProcedures = storedProcedures
                    };

                    response.SetResult(result);
                    return response;
                }
                catch (Exception ex)
                {
                    response.AddError($"Error retrieving database objects: {ex.Message}");
                    return response;
                }
            });
    }

}
