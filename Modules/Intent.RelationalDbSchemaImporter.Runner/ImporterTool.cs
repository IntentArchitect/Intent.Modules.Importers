using System.Text;
using System.Text.Json;
using CliWrap;
using Intent.RelationalDbSchemaImporter.Contracts.Models;
using Intent.Utils;

namespace Intent.RelationalDbSchemaImporter.Runner;

public static class ImporterTool
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = false
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
    
    private static string ToolExecutable => Path.Combine(ToolDirectory, "Intent.SQLSchemaExtractor.dll");

    public static void SetToolDirectory(string toolDirectory)
    {
        ToolDirectory = toolDirectory;
    }
    
    public static StandardResponse<TResult> Run<TResult>(string command, object payloadObject)
    {
        var payloadJson = JsonSerializer.Serialize(payloadObject, SerializerOptions);

        string? responseLine = null;
        var errorString = new StringBuilder();
        
        Cli.Wrap("dotnet")
            .WithArguments([ToolExecutable, command, "--payload", payloadJson])
            .WithWorkingDirectory(ToolDirectory)
            .WithStandardOutputPipe(PipeTarget.ToDelegate((line, ct) =>
            {
                Logging.Log.Debug($"Output: {line}");
                if (!line.StartsWith('{'))
                {
                    return Task.CompletedTask;
                }

                responseLine = line;

                return Task.CompletedTask;
            }))
            .WithStandardErrorPipe(PipeTarget.ToDelegate((line, ct) =>
            {
                Logging.Log.Debug($"Error: {line}");
                errorString.AppendLine(line);
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

        if (errorString.Length > 0)
        {
            throw new InvalidOperationException(errorString.ToString());
        }
        
        if (responseLine is not null)
        {
            var responseObj = JsonSerializer.Deserialize<StandardResponse<TResult>>(responseLine, SerializerOptions)!;
            return responseObj;
        }
        
        throw new InvalidOperationException("No response received.");
    }
}