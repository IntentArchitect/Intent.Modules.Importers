using System.Text.Json;
using System.Text.Json.Serialization;

namespace Intent.Modules.Rdbms.Importer.Tasks.Helpers;

internal static class SerializationHelper
{
    public static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
        // Settings files written by earlier versions of this module used the default (PascalCase)
        // naming policy, so reads must tolerate either casing.
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public static readonly JsonSerializerOptions IndentedSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() },
        WriteIndented = true
    };
}
