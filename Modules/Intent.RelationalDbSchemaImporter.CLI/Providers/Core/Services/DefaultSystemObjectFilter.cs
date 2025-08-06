using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.RelationalDbSchemaImporter.CLI.Providers.Core.Services;

internal abstract class SystemObjectFilterBase
{
    public abstract bool IsSystemObject(string? schema, string? name);
}

internal class DefaultSystemObjectFilter : SystemObjectFilterBase
{
    private static readonly HashSet<string> MigrationTables = new(StringComparer.OrdinalIgnoreCase)
    {
        "__MigrationHistory",
        "__EFMigrationsHistory"
    };
    private static readonly HashSet<string> SystemSchemas = new(StringComparer.OrdinalIgnoreCase)
    {
        "INFORMATION_SCHEMA"
    };
    
    public override bool IsSystemObject(string? schema, string? name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return true;
        }

        return (schema is not null && SystemSchemas.Contains(schema))
               || MigrationTables.Contains(name);
    }
} 