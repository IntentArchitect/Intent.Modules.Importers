using System;
using System.CommandLine;
using System.Reflection;
using System.Threading.Tasks;

namespace Intent.SQLSchemaExtractor;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("Intent SQL Schema Extractor - API-style commands with JSON payloads");
        
        // Register all commands
        rootCommand.Subcommands.Add(Commands.CreateImportSchemaCommand());
        rootCommand.Subcommands.Add(Commands.CreateListStoredProceduresCommand());
        rootCommand.Subcommands.Add(Commands.CreateTestConnectionCommand());
        rootCommand.Subcommands.Add(Commands.CreateRetrieveDatabaseObjectsCommand());
        
        Console.WriteLine($"Intent SQL Schema Extractor version {Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion}");

        return await rootCommand.Parse(args).InvokeAsync();
    }
}
