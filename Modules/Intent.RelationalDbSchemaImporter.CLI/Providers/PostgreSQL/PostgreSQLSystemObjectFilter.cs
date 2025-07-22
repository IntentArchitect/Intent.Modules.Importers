using System;
using System.Linq;
using Intent.RelationalDbSchemaImporter.CLI.Providers.Core.Services;

namespace Intent.RelationalDbSchemaImporter.CLI.Providers.PostgreSQL;

/// <summary>
/// PostgreSQL-specific system object filter with additional PostgreSQL system schemas
/// </summary>
internal class PostgreSQLSystemObjectFilter : DefaultSystemObjectFilter
{
    /// <summary>
    /// Override system object detection to include PostgreSQL-specific system schemas
    /// </summary>
    public override bool IsSystemObject(string? schema, string? name)
    {
        if (string.IsNullOrEmpty(name)) return true;

        var postgresSystemSchemas = new[]
        {
            "information_schema", "pg_catalog", "pg_toast", "pg_temp", "pg_toast_temp",
            "sys", "INFORMATION_SCHEMA" // Include generic system schemas too
        };
        var systemTables = new[] { "sysdiagrams", "__MigrationHistory", "__EFMigrationsHistory" };

        return postgresSystemSchemas.Contains(schema, StringComparer.OrdinalIgnoreCase) ||
               systemTables.Contains(name, StringComparer.OrdinalIgnoreCase) ||
               (name?.StartsWith("pg_", StringComparison.OrdinalIgnoreCase) == true);
    }
} 