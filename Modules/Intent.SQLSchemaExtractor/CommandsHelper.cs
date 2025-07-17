using System;
using System.CommandLine;
using System.Text.Json;
using System.Text.Json.Serialization;
using Intent.Modules.Common.Templates;
using Intent.RelationalDbSchemaImporter.Contracts.Models;

namespace Intent.SQLSchemaExtractor;

internal static partial class Commands
{
    private static readonly JsonSerializerOptions DeserializationOptions = new()
    {
        Converters = { new JsonStringEnumConverter() }
    };

    private static string GetOptionName(string propertyName) => $"--{propertyName.ToKebabCase()}";
    
    private static Command CreateStandardCommand<TResult>(string name, string description, Func<string, StandardResponse<TResult>, StandardResponse<TResult>> executeFunc)
    {
        var command = new Command(name, description);
        
        var payloadOption = CreatePayloadOption();
        var prettyPrintOption = CreatePrettyPrintOption();
        
        command.Options.Add(payloadOption);
        command.Options.Add(prettyPrintOption);
        
        command.SetAction(parseResult =>
        {
            var prettyPrint = parseResult.GetValue<bool>(GetOptionName("pretty-print"));
            var response = new StandardResponse<TResult>();
            try
            {
                var payload = parseResult.GetValue<string>(GetOptionName("payload"))!;
                
                var execResponse = executeFunc(payload, response);
                
                var json = SerializeResponse(execResponse, prettyPrint);
                Console.WriteLine(json);
            }
            catch (Exception ex)
            {
                response.AddError(ex.Message);
                var json = SerializeResponse(response, prettyPrint);
                Console.WriteLine(json);
            }
        });
        
        return command;
    }

    private static Option<string> CreatePayloadOption()
    {
        var option = new Option<string>(GetOptionName("payload")) { Required = true };
        option.Description = "JSON payload";
        return option;
    }

    private static Option<bool> CreatePrettyPrintOption()
    {
        var option = new Option<bool>(GetOptionName("pretty-print"));
        option.Description = "Format JSON output for readability";
        return option;
    }

    private static string SerializeResponse<TResult>(StandardResponse<TResult> response, bool prettyPrint)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = prettyPrint,
            Converters = { new JsonStringEnumConverter() }
        };
        return JsonSerializer.Serialize(response, options);
    }

    private static TRequest? DeserializeRequest<TRequest>(string jsonPayload, StandardResponse response) where TRequest : class
    {
        try
        {
            var request = JsonSerializer.Deserialize<TRequest>(jsonPayload, DeserializationOptions);
            if (request == null)
            {
                response.AddError("Invalid JSON payload");
                return null;
            }
            return request;
        }
        catch (JsonException ex)
        {
            response.AddError($"JSON deserialization error: {ex.Message}");
            return null;
        }
    }

    private static bool ValidateConnectionString(string? connectionString, StandardResponse response)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            response.AddError("ConnectionString is required");
            return false;
        }
        return true;
    }
}