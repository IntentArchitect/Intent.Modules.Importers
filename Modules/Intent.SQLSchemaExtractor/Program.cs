using System;
using System.CommandLine;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Modules.Common.Templates;
using Intent.SQLSchemaExtractor.Annotators;
using Intent.SQLSchemaExtractor.ExtensionMethods;
using Intent.SQLSchemaExtractor.Extractors;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Intent.SQLSchemaExtractor;

public class Program
{
    private static string GetOptionName(string propertyName) => $"--{propertyName.ToKebabCase()}";

    public static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand(
            "The Intent SQL Schema Extractor tool can be used to create / update ans Intent Architect Domain Package" +
            "  based on a  SQL Schema.")
        {
            new Option<FileInfo>(
                GetOptionName(nameof(ImportConfiguration.ConfigFile)),
                "Path to a JSON formatted file containing options to use for execution of " +
                             "this tool as an alternative to using command line options. The " +
                             $"{GetOptionName(nameof(ImportConfiguration.GenerateConfigFile))} option can be used to " +
                             "generate a file with all the possible fields populated with null."),
            new Option<bool>(
                GetOptionName(nameof(ImportConfiguration.GenerateConfigFile)),
                $"Scaffolds into the current working directory a \"config.json\" for use with the " +
                             $"{GetOptionName(nameof(ImportConfiguration.ConfigFile))} option."),
            new Option<string?>(
                GetOptionName(nameof(ImportConfiguration.ConnectionString)),
                "Connection string for connecting to the database to import the schema from. "),
            new Option<string?>(
                GetOptionName(nameof(ImportConfiguration.PackageFileName)),
                "The file name of the Intent Domain Package into which to synchronize the metadata."),
            new Option<string?>(
                GetOptionName(nameof(ImportConfiguration.ImportFilterFilePath)),
                $"Path to import filter file (may be relative to {nameof(ImportConfiguration.PackageFileName)} file)"),
            new Option<string?>(
                GetOptionName(nameof(ImportConfiguration.SerializedConfig)),
                "JSON string representing a serialized configuration file."),
        };
        
        rootCommand.SetAction(parseResult =>
            {
                try
                {
                    var serializerOptions = new JsonSerializerOptions
                    {
                        Converters = { new JsonStringEnumConverter() },
                        WriteIndented = true
                    };

                    var configFile = parseResult.GetValue<FileInfo>(GetOptionName(nameof(ImportConfiguration.ConfigFile)));
                    var generateConfigFile = parseResult.GetValue<bool>(GetOptionName(nameof(ImportConfiguration.GenerateConfigFile)));
                    var connectionString = parseResult.GetValue<string?>(GetOptionName(nameof(ImportConfiguration.ConnectionString)));
                    var packageFileName = parseResult.GetValue<string?>(GetOptionName(nameof(ImportConfiguration.PackageFileName)));
                    var importFilterFilePath = parseResult.GetValue<string?>(GetOptionName(nameof(ImportConfiguration.ImportFilterFilePath)));
                    var serializedConfig = parseResult.GetValue<string?>(GetOptionName(nameof(ImportConfiguration.SerializedConfig)));

                    if (generateConfigFile)
                    {
                        var path = Path.Join(Environment.CurrentDirectory, "config.json");
                        Console.WriteLine($"Writing {path}...");
                        File.WriteAllBytes(path, JsonSerializer.SerializeToUtf8Bytes(new ImportConfiguration(), serializerOptions));
                        Console.WriteLine("Done.");
                        return;
                    }

                    ImportConfiguration config;
                    if (serializedConfig != null)
                    {
                        config = JsonSerializer.Deserialize<ImportConfiguration>(serializedConfig, serializerOptions)
                                 ?? throw new Exception($"Parsing of serialized-config returned null.");
                    }
                    else if (configFile != null)
                    {
                        config = JsonSerializer.Deserialize<ImportConfiguration>(File.ReadAllText(configFile.FullName), serializerOptions)
                                 ?? throw new Exception($"Parsing of \"{configFile.FullName}\" returned null.");
                    }
                    else
                    {
                        config = new ImportConfiguration();
                    }

                    if (connectionString != null)
                        config.ConnectionString = connectionString;
                    if (packageFileName != null)
                        config.PackageFileName = packageFileName;
                    if (importFilterFilePath != null)
                        config.ImportFilterFilePath = importFilterFilePath;

                    if (!string.IsNullOrWhiteSpace(config.ImportFilterFilePath) &&
                        !Path.IsPathRooted(config.ImportFilterFilePath))
                    {
                        config.ImportFilterFilePath = Path.Combine(Path.GetDirectoryName(config.PackageFileName)!, config.ImportFilterFilePath);
                    }
                    
                    if(!config.ValidateFilterFile())
                    {
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(config.ConnectionString))
                        throw new Exception($"{GetOptionName(nameof(ImportConfiguration.ConnectionString))} is mandatory or a --config-file with a connection string.");

                    if (string.IsNullOrWhiteSpace(config.PackageFileName))
                        throw new Exception($"{GetOptionName(nameof(ImportConfiguration.PackageFileName))} is mandatory or a --config-file with a package file name.");

                    if (config.StoredProcedureType == StoredProcedureType.Default)
                    {
                        config.StoredProcedureType = StoredProcedureType.StoredProcedureElement;
                    }
                    
                    Run(config);
                }
                catch (Exception exception)
                {
                    Logging.LogError($"{exception.Message}");
                    Console.WriteLine(".");
                    throw;
                }
            });

        var connectionOption = new Option<string>("--connection", "SQL Server connection string.") { Required = true };
        
        var listStoredProcCommand = new Command("list-stored-proc", "Returns a list of stored procedures in the database.");
        listStoredProcCommand.Options.Add(connectionOption);
        listStoredProcCommand.SetAction((parseResult, cancellationToken) => ListProceduresAsync(parseResult.GetValue<string>("--connection")));
        rootCommand.Subcommands.Add(listStoredProcCommand);

        var testConnectionCommand = new Command("test-connection", "Tests the connection to the database.");
        testConnectionCommand.Options.Add(connectionOption);
        testConnectionCommand.SetAction((parseResult, cancellationToken) => TestConnectionAsync(parseResult.GetValue<string>("--connection")));
        rootCommand.Subcommands.Add(testConnectionCommand);
        
        var extractMetadataCommand = new Command("extract-metadata", "Extracts database metadata (tables, views, stored procedures) as JSON.");
        extractMetadataCommand.Options.Add(connectionOption);
        extractMetadataCommand.SetAction((parseResult, cancellationToken) => ExtractMetadataAsync(parseResult.GetValue<string>("--connection")));
        rootCommand.Subcommands.Add(extractMetadataCommand);
        
        Console.WriteLine($"{rootCommand.Name} version {Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion}");

        return await rootCommand.Parse(args).InvokeAsync();
    }

    public static void Run(ImportConfiguration config)
    {
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

        Console.WriteLine("Saving package...");
        package.Save();

        Console.WriteLine("Package saved successfully.");
        Console.WriteLine();
    }

    public static async Task<int> ListProceduresAsync(string connectionString)
    {
        try
        {
            await using var connection = new SqlConnection(connectionString);
            connection.Open();
            var server = new Server(new ServerConnection(connection));
            var db = server.Databases[connection.Database];
            var storedProcs = db.StoredProcedures
                .OfType<StoredProcedure>()
                .Where(x => x.Schema != "sys")
                .ToList();
            Console.WriteLine($"Stored Procedures: {storedProcs.Count}");
            foreach (var storedProc in storedProcs)
            {
                Console.WriteLine($"{storedProc.Schema}.{storedProc.Name}");
            }
            Console.WriteLine(".");
            return 0;
        }
        catch (Exception ex)
        {
            Logging.LogError(ex.Message);
            Console.WriteLine(".");
            return 1;
        }
    }
    
    private static async Task<int> TestConnectionAsync(string connectionString)
    {
        try
        {
            await using var connection = new SqlConnection(connectionString);
            connection.Open();
            var server = new Server(new ServerConnection(connection));
            var db = server.Databases[connection.Database];
            db.ExecuteWithResults("SELECT 1");
            Console.WriteLine("Successfully established a connection.");
            return 0;
        }
        catch (Exception ex)
        {
            Logging.LogError(ex.Message);
            Console.WriteLine(".");
            return 1;
        }
    }
    
    public static async Task<int> ExtractMetadataAsync(string connectionString)
    {
        try
        {
            await using var connection = new SqlConnection(connectionString);
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

            var metadata = new
            {
                tables,
                views,
                storedProcedures
            };
            
            var options = new JsonSerializerOptions
            {
                WriteIndented = false
            };
            
            var json = JsonSerializer.Serialize(metadata, options);
            json = json.Replace("\r", string.Empty).Replace("\n", string.Empty);
            Console.WriteLine(json);
            return 0;
        }
        catch (Exception ex)
        {
            Logging.LogError(ex.Message);
            Console.WriteLine(".");
            return 1;
        }
    }
}