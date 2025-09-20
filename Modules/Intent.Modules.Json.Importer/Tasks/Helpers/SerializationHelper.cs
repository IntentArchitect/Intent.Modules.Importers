using System.Text.Json;
using System.Text.Json.Serialization;

namespace Intent.Modules.Json.Importer.Tasks.Helpers;

internal static class SerializationHelper
{
    public static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };
}