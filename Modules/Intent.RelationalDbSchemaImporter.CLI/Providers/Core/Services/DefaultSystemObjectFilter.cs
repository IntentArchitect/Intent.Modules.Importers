using System;
using System.Linq;

namespace Intent.RelationalDbSchemaImporter.CLI.Providers.Core.Services;

internal abstract class SystemObjectFilterBase
{
    public abstract bool IsSystemObject(string? schema, string? name);
}

internal class DefaultSystemObjectFilter : SystemObjectFilterBase
{
    public override bool IsSystemObject(string? schema, string? name)
    {
        if (string.IsNullOrEmpty(name)) return true;
        
        var systemSchemas = new[] { "sys", "INFORMATION_SCHEMA", "information_schema", "pg_catalog", "pg_toast" };
        var systemTables = new[] { "sysdiagrams", "__MigrationHistory", "__EFMigrationsHistory" };
        
        return systemSchemas.Contains(schema, StringComparer.OrdinalIgnoreCase) ||
               systemTables.Contains(name, StringComparer.OrdinalIgnoreCase);
    }
} 