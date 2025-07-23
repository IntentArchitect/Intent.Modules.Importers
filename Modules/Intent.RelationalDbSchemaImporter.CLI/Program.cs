using System.CommandLine;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Intent.RelationalDbSchemaImporter.CLI;

/// <summary>
/// RPC-style backend tool for database schema extraction used by Intent Architect importer modules.
/// This tool is separated into its own executable due to native library dependencies 
/// (Microsoft.Data.SqlClient, Npgsql) that cannot be included in Intent Architect modules.
/// </summary>
public class Program
{
    public static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand(
            """
            Intent Relational Database Schema Importer - RPC backend for Intent Architect modules.
            This tool provides database schema extraction services through JSON-based commands.
            """);
        
        rootCommand.Subcommands.Add(Commands.CreateImportSchemaCommand());
        rootCommand.Subcommands.Add(Commands.CreateListStoredProceduresCommand());
        rootCommand.Subcommands.Add(Commands.CreateTestConnectionCommand());
        rootCommand.Subcommands.Add(Commands.CreateRetrieveDatabaseObjectsCommand());

        return await rootCommand.Parse(args).InvokeAsync();
    }
}