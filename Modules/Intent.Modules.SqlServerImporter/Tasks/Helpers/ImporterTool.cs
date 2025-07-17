using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CliWrap;
using Intent.Modules.SqlServerImporter.Tasks.Models;
using Intent.RelationalDbSchemaImporter.Contracts.Models;
using Intent.Utils;

namespace Intent.Modules.SqlServerImporter.Tasks.Helpers;

internal static class ImporterTool
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = false
    };
    
    private static readonly string ToolDirectory = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(typeof(ImporterTool).Assembly.Location)!, "../content/tool"));
    private static readonly string ToolExecutable = Path.Combine(ToolDirectory, "Intent.SQLSchemaExtractor.dll");
    
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

        if (responseLine is not null)
        {
            var responseObj = JsonSerializer.Deserialize<StandardResponse<TResult>>(responseLine, SerializerOptions)!;
            return responseObj;
        }

        if (errorString.Length > 0)
        {
            throw new InvalidOperationException(errorString.ToString());
        }
        
        throw new InvalidOperationException("No response received.");
    }
}