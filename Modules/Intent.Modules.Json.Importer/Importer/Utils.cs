using Intent.Modules.Common.Templates;

namespace Intent.Modules.Json.Importer.Importer;

public static class Utils
{
    public static string Casing(JsonConfig config, string name)
    {
        return config.CasingConvention == CasingConvention.AsIs ? name : name.ToPascalCase();
    }
}
