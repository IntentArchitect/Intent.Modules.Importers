using System;
using System.Collections.Generic;
using System.Linq;
using Intent.RelationalDbSchemaImporter.CLI.Providers.Core.Services;

namespace Intent.RelationalDbSchemaImporter.CLI.Providers.SqlServer;

internal class SqlServerSystemObjectFilter : DefaultSystemObjectFilter
{
    private static readonly HashSet<string> SqlServerSystemSchemas = new(StringComparer.OrdinalIgnoreCase)
    {
        "sys"
    };
    private static readonly HashSet<string> SqlServerSystemTables = new(StringComparer.OrdinalIgnoreCase)
    {
        "sysdiagrams"
    };
    
    public override bool IsSystemObject(string? schema, string? name)
    {
        if (string.IsNullOrEmpty(name) || base.IsSystemObject(schema, name))
        {
            return true;
        }

        return (schema is not null && SqlServerSystemSchemas.Contains(schema)) ||
               SqlServerSystemTables.Contains(name);
    }
} 