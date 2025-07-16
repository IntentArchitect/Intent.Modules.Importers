using System;
using System.CommandLine;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Modules.Common.Templates;
using Intent.SQLSchemaExtractor.Annotators;
using Intent.SQLSchemaExtractor.ExtensionMethods;
using Intent.SQLSchemaExtractor.Extractors;
using Intent.SQLSchemaExtractor.Models;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace Intent.SQLSchemaExtractor;

public static class Commands
{
    private static string GetOptionName(string propertyName) => $"--{propertyName.ToKebabCase()}";

    public static Command CreateImportSchemaCommand()
    {
        var command = new Command("import-schema", "Imports database schema into Intent package");
        var payloadOption = new Option<string>("--payload") { Required = true };
        payloadOption.Description = "JSON payload";
        var prettyPrintOption = new Option<bool>("--pretty-print");
        prettyPrintOption.Description = "Format JSON output for readability";
        
        command.Options.Add(payloadOption);
        command.Options.Add(prettyPrintOption);
        
        command.SetAction(parseResult =>
        {
            var payload = parseResult.GetValue<string>("--payload");
            var prettyPrint = parseResult.GetValue<bool>("--pretty-print");
            
            var response = ExecuteImportSchema(payload);
            var json = SerializeResponse(response, prettyPrint);
            Console.WriteLine(json);
        });
        
        return command;
    }

    public static Command CreateListStoredProcsCommand()
    {
        var command = new Command("list-stored-procs", "Returns a list of stored procedures in the database");
        var payloadOption = new Option<string>("--payload") { Required = true };
        payloadOption.Description = "JSON payload";
        var prettyPrintOption = new Option<bool>("--pretty-print");
        prettyPrintOption.Description = "Format JSON output for readability";
        
        command.Options.Add(payloadOption);
        command.Options.Add(prettyPrintOption);
        
        command.SetAction(parseResult =>
        {
            var payload = parseResult.GetValue<string>("--payload");
            var prettyPrint = parseResult.GetValue<bool>("--pretty-print");
            
            var response = ExecuteListStoredProcs(payload);
            var json = SerializeResponse(response, prettyPrint);
            Console.WriteLine(json);
        });
        
        return command;
    }

    public static Command CreateTestConnectionCommand()
    {
        var command = new Command("test-connection", "Tests the connection to the database");
        var payloadOption = new Option<string>("--payload") { Required = true };
        payloadOption.Description = "JSON payload";
        var prettyPrintOption = new Option<bool>("--pretty-print");
        prettyPrintOption.Description = "Format JSON output for readability";
        
        command.Options.Add(payloadOption);
        command.Options.Add(prettyPrintOption);
        
        command.SetAction(parseResult =>
        {
            var payload = parseResult.GetValue<string>("--payload");
            var prettyPrint = parseResult.GetValue<bool>("--pretty-print");
            
            var response = ExecuteTestConnection(payload);
            var json = SerializeResponse(response, prettyPrint);
            Console.WriteLine(json);
        });
        
        return command;
    }

    public static Command CreateRetrieveDatabaseObjectsCommand()
    {
        var command = new Command("retrieve-database-objects", "Extracts database metadata (tables, views, stored procedures) as JSON");
        var payloadOption = new Option<string>("--payload") { Required = true };
        payloadOption.Description = "JSON payload";
        var prettyPrintOption = new Option<bool>("--pretty-print");
        prettyPrintOption.Description = "Format JSON output for readability";
        
        command.Options.Add(payloadOption);
        command.Options.Add(prettyPrintOption);
        
        command.SetAction(parseResult =>
        {
            var payload = parseResult.GetValue<string>("--payload");
            var prettyPrint = parseResult.GetValue<bool>("--pretty-print");
            
            var response = ExecuteRetrieveDatabaseObjects(payload);
            var json = SerializeResponse(response, prettyPrint);
            Console.WriteLine(json);
        });
        
        return command;
    }

    private static string SerializeResponse(StandardResponse response, bool prettyPrint)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = prettyPrint,
            Converters = { new JsonStringEnumConverter() }
        };
        return JsonSerializer.Serialize(response, options);
    }

    private static StandardResponse ExecuteImportSchema(string jsonPayload)
    {
        var response = new StandardResponse();
        try
        {
            var request = JsonSerializer.Deserialize<ImportSchemaRequest>(jsonPayload, new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() }
            });

            if (request == null)
            {
                response.AddError("Invalid JSON payload");
                return response;
            }

            // Convert request to ImportConfiguration
            var config = new ImportConfiguration
            {
                ConnectionString = request.ConnectionString,
                PackageFileName = request.PackageFileName,
                ImportFilterFilePath = request.ImportFilterFilePath,
                ApplicationId = request.ApplicationId,
                EntityNameConvention = request.EntityNameConvention,
                TableStereotype = request.TableStereotype,
                TypesToExport = request.TypesToExport,
                StoredProcedureType = request.StoredProcedureType,
                RepositoryElementId = request.RepositoryElementId,
                StoredProcNames = request.StoredProcNames
            };

            if (string.IsNullOrWhiteSpace(config.ConnectionString))
            {
                response.AddError("ConnectionString is required");
                return response;
            }

            if (string.IsNullOrWhiteSpace(config.PackageFileName))
            {
                response.AddError("PackageFileName is required");
                return response;
            }

            if (!string.IsNullOrWhiteSpace(config.ImportFilterFilePath) &&
                !Path.IsPathRooted(config.ImportFilterFilePath))
            {
                config.ImportFilterFilePath = Path.Combine(Path.GetDirectoryName(config.PackageFileName)!, config.ImportFilterFilePath);
            }
            
            if (!config.ValidateFilterFile())
            {
                response.AddError("Import filter file validation failed");
                return response;
            }

            if (config.StoredProcedureType == StoredProcedureType.Default)
            {
                config.StoredProcedureType = StoredProcedureType.StoredProcedureElement;
            }

            // Execute the import
            using var connection = new SqlConnection(config.ConnectionString);
            connection.Open();
            var server = new Server(new ServerConnection(connection));
            var db = server.Databases[connection.Database];
            var extractor = new SqlServerSchemaExtractor(config, db, server);

            var package = extractor.BuildPackageModel(config.PackageFileName!, new SchemaExtractorEventManager
            {
                OnTableHandlers = new[]
                {
                    RdbmsSchemaAnnotator.ApplyTableDetails
                },
                OnViewHandlers = new[]
                {
                    RdbmsSchemaAnnotator.ApplyViewDetails
                },
                OnTableColumnHandlers = new[]
                {
                    RdbmsSchemaAnnotator.ApplyPrimaryKey,
                    RdbmsSchemaAnnotator.ApplyColumnDetails,
                    RdbmsSchemaAnnotator.ApplyTextConstraint,
                    RdbmsSchemaAnnotator.ApplyDecimalConstraint,
                    RdbmsSchemaAnnotator.ApplyDefaultConstraint,
                    RdbmsSchemaAnnotator.ApplyComputedValue
                },
                OnViewColumnHandlers = new[]
                {
                    RdbmsSchemaAnnotator.ApplyColumnDetails,
                    RdbmsSchemaAnnotator.ApplyTextConstraint,
                    RdbmsSchemaAnnotator.ApplyDecimalConstraint
                },
                OnIndexHandlers = new[]
                {
                    RdbmsSchemaAnnotator.ApplyIndex
                },
                OnStoredProcedureHandlers = new[]
                {
                    RdbmsSchemaAnnotator.ApplyStoredProcedureSettings
                }
            });

            package.Name = Path.GetFileNameWithoutExtension(package.Name);
            package.References.Add(new PackageReferenceModel
            {
                PackageId = "870ad967-cbd4-4ea9-b86d-9c3a5d55ea67",
                Name = "Intent.Common.Types",
                Module = "Intent.Common.Types",
                IsExternal = true
            });
            package.References.Add(new PackageReferenceModel
            {
                PackageId = "AF8F3810-745C-42A2-93C8-798860DC45B1",
                Name = "Intent.Metadata.RDBMS",
                Module = "Intent.Metadata.RDBMS",
                IsExternal = true
            });
            package.References.Add(new PackageReferenceModel
            {
                PackageId = "a9d2a398-04e4-4300-9fbb-768568c65f9e",
                Name = "Intent.EntityFrameworkCore",
                Module = "Intent.EntityFrameworkCore",
                IsExternal = true
            });

            if (extractor.Statistics.StoredProcedureCount > 0 || config.ExportStoredProcedures())
            {
                package.References.Add(new PackageReferenceModel
                {
                    PackageId = "5869084c-2a08-4e40-a5c9-ff26220470c8",
                    Name = "Intent.EntityFrameworkCore.Repositories",
                    Module = "Intent.EntityFrameworkCore.Repositories",
                    IsExternal = true
                });
            }

            package.Save();

            var result = new ImportSchemaResult
            {
                PackageName = package.Name,
                PackageFilePath = config.PackageFileName,
                TablesImported = extractor.Statistics.TableCount,
                ViewsImported = extractor.Statistics.ViewCount,
                StoredProceduresImported = extractor.Statistics.StoredProcedureCount,
                IndexesImported = extractor.Statistics.IndexCount
            };

            response.SetResult(result);
            return response;
        }
        catch (Exception ex)
        {
            response.AddError(ex.Message);
            return response;
        }
    }

    private static StandardResponse ExecuteListStoredProcs(string jsonPayload)
    {
        var response = new StandardResponse();
        try
        {
            var request = JsonSerializer.Deserialize<ConnectionRequest>(jsonPayload);
            if (request == null)
            {
                response.AddError("Invalid JSON payload");
                return response;
            }

            if (string.IsNullOrWhiteSpace(request.ConnectionString))
            {
                response.AddError("ConnectionString is required");
                return response;
            }

            using var connection = new SqlConnection(request.ConnectionString);
            connection.Open();
            var server = new Server(new ServerConnection(connection));
            var db = server.Databases[connection.Database];
            var storedProcs = db.StoredProcedures
                .OfType<StoredProcedure>()
                .Where(x => x.Schema != "sys")
                .Select(sp => $"{sp.Schema}.{sp.Name}")
                .OrderBy(name => name)
                .ToList();

            response.SetResult(storedProcs);
            return response;
        }
        catch (Exception ex)
        {
            response.AddError(ex.Message);
            return response;
        }
    }

    private static StandardResponse ExecuteTestConnection(string jsonPayload)
    {
        var response = new StandardResponse();
        try
        {
            var request = JsonSerializer.Deserialize<ConnectionRequest>(jsonPayload);
            if (request == null)
            {
                response.AddError("Invalid JSON payload");
                return response;
            }

            if (string.IsNullOrWhiteSpace(request.ConnectionString))
            {
                response.AddError("ConnectionString is required");
                return response;
            }

            using var connection = new SqlConnection(request.ConnectionString);
            connection.Open();
            var server = new Server(new ServerConnection(connection));
            var db = server.Databases[connection.Database];
            db.ExecuteWithResults("SELECT 1");

            var result = new ConnectionTestResult
            {
                IsSuccessful = true,
                DatabaseName = db.Name,
                ServerName = server.Name
            };

            response.SetResult(result);
            return response;
        }
        catch (Exception ex)
        {
            var result = new ConnectionTestResult
            {
                IsSuccessful = false,
                DatabaseName = string.Empty,
                ServerName = string.Empty
            };
            response.SetResult(result);
            response.AddError(ex.Message);
            return response;
        }
    }

    private static StandardResponse ExecuteRetrieveDatabaseObjects(string jsonPayload)
    {
        var response = new StandardResponse();
        try
        {
            var request = JsonSerializer.Deserialize<ConnectionRequest>(jsonPayload);
            if (request == null)
            {
                response.AddError("Invalid JSON payload");
                return response;
            }

            if (string.IsNullOrWhiteSpace(request.ConnectionString))
            {
                response.AddError("ConnectionString is required");
                return response;
            }

            using var connection = new SqlConnection(request.ConnectionString);
            connection.Open();
            var server = new Server(new ServerConnection(connection));
            var db = server.Databases[connection.Database];
            
            var tables = db.Tables
                .OfType<Table>()
                .Where(x => x.Schema != "sys")
                .Select(t => $"{t.Schema}.{t.Name}")
                .OrderBy(name => name)
                .ToList();
                
            var views = db.Views
                .OfType<View>()
                .Where(x => x.Schema != "sys" && x.Schema != "INFORMATION_SCHEMA")
                .Select(v => $"{v.Schema}.{v.Name}")
                .OrderBy(name => name)
                .ToList();
                
            var storedProcedures = db.StoredProcedures
                .OfType<StoredProcedure>()
                .Where(x => x.Schema != "sys")
                .Select(sp => $"{sp.Schema}.{sp.Name}")
                .OrderBy(name => name)
                .ToList();

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
            response.AddError(ex.Message);
            return response;
        }
    }
}
