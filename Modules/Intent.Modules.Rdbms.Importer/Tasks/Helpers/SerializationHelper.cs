using System.Text.Json;
using System.Text.Json.Serialization;

namespace Intent.Modules.Rdbms.Importer.Tasks.Helpers;

internal static class SerializationHelper
{
    public static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };
}