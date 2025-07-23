using System;
using System.CommandLine;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Intent.Modules.Common.Templates;
using Intent.RelationalDbSchemaImporter.CLI.Providers;
using Intent.RelationalDbSchemaImporter.CLI.Providers.Core;
using Intent.RelationalDbSchemaImporter.CLI.Services;
using Intent.RelationalDbSchemaImporter.Contracts.Commands;
using Intent.RelationalDbSchemaImporter.Contracts.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Intent.RelationalDbSchemaImporter.CLI;

internal static partial class Commands
{
    private static readonly JsonSerializerOptions DeserializationOptions = new()
    {
        Converters = { new JsonStringEnumConverter() },
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// Converts a property name to a kebab-case string.
    /// </summary>
    /// <param name="propertyName">The property name to convert.</param>
    /// <returns>The kebab-case string.</returns>
    private static string GetOptionName(string propertyName) => $"--{propertyName.ToKebabCase()}";
    
    /// <summary>
    /// Creates a standard command.
    /// </summary>
    /// <typeparam name="TResult">Type returned by the command implementation.</typeparam>
    /// <param name="name">The name of the command.</param>
    /// <param name="description">The description of the command.</param>
    /// <param name="executeFunc">Command implementation passed through as a delegate.</param>
    private static Command CreateStandardCommand<TResult>(
        string name, 
        string description, 
        Func<string, StandardResponse<TResult>, CancellationToken, Task<StandardResponse<TResult>>> executeFunc)
    {
        var command = new Command(name, description);
        
        var payloadOption = CreatePayloadOption();
        var prettyPrintOption = CreatePrettyPrintOption();
        
        command.Options.Add(payloadOption);
        command.Options.Add(prettyPrintOption);
        
        command.SetAction(async (parseResult, cancellationToken) =>
        {
            var prettyPrint = parseResult.GetValue<bool>(GetOptionName("pretty-print"));
            var response = new StandardResponse<TResult>();
            try
            {
                var payload = parseResult.GetValue<string>(GetOptionName("payload"))!;

                var execResponse = await executeFunc(payload, response, cancellationToken);
                
                ConsoleOutput.JsonOutput(execResponse, prettyPrint);
            }
            catch (Exception ex)
            {
                response.AddError(ex.Message);
                ConsoleOutput.JsonOutput(response, prettyPrint);
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

    /// <summary>
    /// Deserializes a JSON payload into a request object.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request object.</typeparam>
    /// <param name="jsonPayload">The JSON payload to deserialize.</param>
    /// <param name="response">The response object to add errors to.</param>
    /// <returns>The deserialized request object.</returns>
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

    /// <summary>
    /// Validates that the connection string is not null or empty.
    /// </summary>
    /// <param name="connectionString">The connection string to validate.</param>
    /// <param name="response">The response object to add errors to.</param>
    /// <returns>True if the connection string is valid, false otherwise.</returns>
    private static bool ValidateConnectionString(string? connectionString, StandardResponse response)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            response.AddError("ConnectionString is required");
            return false;
        }
        return true;
    }
    
    /// <summary>
    /// Validates that the database type is specified and supported.
    /// </summary>
    /// <param name="databaseType">The database type to validate.</param>
    /// <param name="response">The response object to add errors to.</param>
    /// <returns>True if the database type is valid, false otherwise.</returns>
    private static bool ValidateDatabaseType(DatabaseType databaseType, StandardResponse response)
    {
        if (databaseType == DatabaseType.Unspecified)
        {
            response.AddError("DatabaseType must be explicitly specified.");
            return false;
        }
        
        var factory = new DatabaseProviderFactory();
        if (!factory.GetSupportedTypes().Contains(databaseType))
        {
            response.AddError($"DatabaseType {databaseType} is not supported. Supported types: {string.Join(", ", factory.GetSupportedTypes())}");
            return false;
        }
        
        return true;
    }
}