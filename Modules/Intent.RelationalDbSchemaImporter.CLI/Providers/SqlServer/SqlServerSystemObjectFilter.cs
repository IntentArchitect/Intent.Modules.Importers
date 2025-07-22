using System;
using System.Collections.Generic;
using System.Linq;
using Intent.RelationalDbSchemaImporter.CLI.Providers.Core.Services;

namespace Intent.RelationalDbSchemaImporter.CLI.Providers.SqlServer;

/// <summary>
/// SQL Server-specific system object filter with additional SQL Server system objects
/// </summary>
internal class SqlServerSystemObjectFilter : DefaultSystemObjectFilter
{
    // Migrated from DatabaseSchemaExtractor._tablesToIgnore
    private readonly List<string> _tablesToIgnore = ["sysdiagrams", "__MigrationHistory", "__EFMigrationsHistory"];
    private readonly List<string> _viewsToIgnore = [];

    /// <summary>
    /// Override system object detection to include SQL Server-specific system objects
    /// Migrated from DatabaseSchemaExtractor filtering logic
    /// </summary>
    public override bool IsSystemObject(string? schema, string? name)
    {
        if (string.IsNullOrEmpty(name)) return true;

        // Check base system objects first (includes sys, INFORMATION_SCHEMA, etc.)
        if (base.IsSystemObject(schema, name))
            return true;

        // SQL Server specific system tables to ignore
        if (_tablesToIgnore.Contains(name, StringComparer.OrdinalIgnoreCase))
            return true;

        // SQL Server specific system views to ignore  
        if (_viewsToIgnore.Contains(name, StringComparer.OrdinalIgnoreCase))
            return true;

        // Additional SQL Server system schema check
        // The base class already handles 'sys' and 'INFORMATION_SCHEMA', but we can add more specific checks
        var sqlServerSystemSchemas = new[] { "sys", "INFORMATION_SCHEMA" };
        if (sqlServerSystemSchemas.Contains(schema, StringComparer.OrdinalIgnoreCase))
            return true;

        return false;
    }
} 