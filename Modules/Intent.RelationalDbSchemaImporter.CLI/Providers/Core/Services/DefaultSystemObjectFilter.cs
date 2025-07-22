using System;
using System.Linq;

namespace Intent.RelationalDbSchemaImporter.CLI.Providers.Core.Services;

internal abstract class SystemObjectFilterBase
{
    public abstract bool IsSystemObject(string? schema, string? name);
}

/// <summary>
/// Base implementation for database system object filtering
/// </summary>
internal class DefaultSystemObjectFilter : SystemObjectFilterBase
{
    /// <summary>
    /// Determines if a database object is a system object that should be filtered out
    /// </summary>
    public override bool IsSystemObject(string? schema, string? name)
    {
        if (string.IsNullOrEmpty(name)) return true;
        
        var systemSchemas = new[] { "sys", "INFORMATION_SCHEMA", "information_schema", "pg_catalog", "pg_toast" };
        var systemTables = new[] { "sysdiagrams", "__MigrationHistory", "__EFMigrationsHistory" };
        
        return systemSchemas.Contains(schema, StringComparer.OrdinalIgnoreCase) ||
               systemTables.Contains(name, StringComparer.OrdinalIgnoreCase);
    }
} 