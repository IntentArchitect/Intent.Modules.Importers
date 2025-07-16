using System;
using System.CommandLine;
using System.Text.Json;
using System.Text.Json.Serialization;
using Intent.Modules.Common.Templates;
using Intent.SQLSchemaExtractor.Models;

namespace Intent.SQLSchemaExtractor;

internal static partial class Commands
{
    private static readonly JsonSerializerOptions DeserializationOptions = new()
    {
        Converters = { new JsonStringEnumConverter() }
    };

    private static string GetOptionName(string propertyName) => $"--{propertyName.ToKebabCase()}";
    
    private static Command CreateStandardCommand(string name, string description, Func<string, StandardResponse> executeFunc)
    {
        var command = new Command(name, description);
        
        var payloadOption = CreatePayloadOption();
        var prettyPrintOption = CreatePrettyPrintOption();
        
        command.Options.Add(payloadOption);
        command.Options.Add(prettyPrintOption);
        
        command.SetAction(parseResult =>
        {
            try
            {
                var payload = parseResult.GetValue<string>(GetOptionName("payload"));
                var prettyPrint = parseResult.GetValue<bool>(GetOptionName("pretty-print"));
                
                var response = executeFunc(payload);
                var json = SerializeResponse(response, prettyPrint);
                Console.WriteLine(json);
            }
            catch (Exception ex)
            {
                var errorResponse = new StandardResponse();
                errorResponse.AddError(ex.Message);
                var json = SerializeResponse(errorResponse, parseResult.GetValue<bool>(GetOptionName("pretty-print")));
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

    private static string SerializeResponse(StandardResponse response, bool prettyPrint)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = prettyPrint,
            Converters = { new JsonStringEnumConverter() }
        };
        return JsonSerializer.Serialize(response, options);
    }

    private static T? DeserializeRequest<T>(string jsonPayload, StandardResponse response) where T : class
    {
        try
        {
            var request = JsonSerializer.Deserialize<T>(jsonPayload, DeserializationOptions);
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