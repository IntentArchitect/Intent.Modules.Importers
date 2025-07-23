using System;
using System.Collections.Generic;
using System.Linq;
using Intent.RelationalDbSchemaImporter.CLI.Providers.Core.Services;

namespace Intent.RelationalDbSchemaImporter.CLI.Providers.SqlServer;

internal class SqlServerSystemObjectFilter : DefaultSystemObjectFilter
{
    private readonly List<string> _tablesToIgnore = ["sysdiagrams", "__MigrationHistory", "__EFMigrationsHistory"];
    private readonly List<string> _viewsToIgnore = [];

    public override bool IsSystemObject(string? schema, string? name)
    {
        if (string.IsNullOrEmpty(name)) return true;

        if (base.IsSystemObject(schema, name))
            return true;

        if (_tablesToIgnore.Contains(name, StringComparer.OrdinalIgnoreCase))
            return true;

        if (_viewsToIgnore.Contains(name, StringComparer.OrdinalIgnoreCase))
            return true;

        var sqlServerSystemSchemas = new[] { "sys", "INFORMATION_SCHEMA" };
        if (sqlServerSystemSchemas.Contains(schema, StringComparer.OrdinalIgnoreCase))
            return true;

        return false;
    }
} 