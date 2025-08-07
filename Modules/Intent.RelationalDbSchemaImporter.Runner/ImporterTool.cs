using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using CliWrap;
using Intent.RelationalDbSchemaImporter.Contracts.Commands;
using Intent.Utils;

namespace Intent.RelationalDbSchemaImporter.Runner;

/// <summary>
/// Wrapper for invoking the Intent.RelationalDbSchemaImporter.CLI tool.
/// </summary>
public static class ImporterTool
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = false,
        Converters = { new JsonStringEnumConverter() },
        PropertyNameCaseInsensitive = true
    };

    private static string? _toolDirectory;
    private static string ToolDirectory
    {
        get
        {
            if (string.IsNullOrWhiteSpace(_toolDirectory))
            {
                throw new InvalidOperationException("Tool directory not set.");
            }
            return _toolDirectory;
        }
        set => _toolDirectory = value;
    }
    
    private static string ToolExecutable => Path.Combine(ToolDirectory, "Intent.RelationalDbSchemaImporter.CLI.dll");

    /// <summary>
    /// Sets the directory containing the Intent.RelationalDbSchemaImporter.CLI tool.
    /// </summary>
    /// <param name="toolDirectory">The directory containing the Intent.RelationalDbSchemaImporter.CLI tool.</param>
    public static void SetToolDirectory(string toolDirectory)
    {
        ToolDirectory = toolDirectory;
    }
    
    /// <summary>
    /// Runs the specified command with the given payload object.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="command">The command to run.</param>
    /// <param name="payloadObject">The payload object to send to the command.</param>
    /// <returns>The result of the command.</returns>
    public static StandardResponse<TResult> Run<TResult>(string command, object payloadObject)
    {
        var payloadJson = JsonSerializer.Serialize(payloadObject, SerializerOptions);

        string? responseLine = null;
        
        Cli.Wrap("dotnet")
            .WithArguments([ToolExecutable, command, "--payload", payloadJson])
            .WithWorkingDirectory(ToolDirectory)
            .WithStandardOutputPipe(PipeTarget.ToDelegate((line, ct) =>
            {
                // Receive the JSON Payload in STD OUT.
                //Logging.Log.Debug($"Output: {line}");
                if (!line.StartsWith('{'))
                {
                    return Task.CompletedTask;
                }

                responseLine = line;

                return Task.CompletedTask;
            }))
            .WithStandardErrorPipe(PipeTarget.ToDelegate((line, ct) =>
            {
                // Receive realtime updates through STD ERR. (doesn't mean it's error feedback)
                Logging.Log.Info(line);
                return Task.CompletedTask;
            }))
            .WithEnvironmentVariables(new Dictionary<string, string?>
            {
                ["DOTNET_CLI_UI_LANGUAGE"] = "en",
                ["DOTNET_ROLL_FORWARD"] = "LatestMajor"
            })
            .ExecuteAsync(p =>
            {
                p.CreateNoWindow = false;
                p.UseShellExecute = false;
                Logging.Log.Debug($"Executing {p.FileName} {p.Arguments}");
            })
            .GetAwaiter()
            .GetResult();
        
        if (responseLine is not null)
        {
            var responseObj = JsonSerializer.Deserialize<StandardResponse<TResult>>(responseLine, SerializerOptions)!;
            return responseObj;
        }
        
        throw new InvalidOperationException("No response received.");
    }
}