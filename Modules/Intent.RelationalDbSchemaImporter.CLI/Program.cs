using System.CommandLine;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Intent.RelationalDbSchemaImporter.CLI;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand(
            $"""
             Intent SQL Schema Extractor - API-style commands with JSON payloads.
             """);
        
        // Register all commands
        rootCommand.Subcommands.Add(Commands.CreateImportSchemaCommand());
        rootCommand.Subcommands.Add(Commands.CreateListStoredProceduresCommand());
        rootCommand.Subcommands.Add(Commands.CreateTestConnectionCommand());
        rootCommand.Subcommands.Add(Commands.CreateRetrieveDatabaseObjectsCommand());

        return await rootCommand.Parse(args).InvokeAsync();
    }
}