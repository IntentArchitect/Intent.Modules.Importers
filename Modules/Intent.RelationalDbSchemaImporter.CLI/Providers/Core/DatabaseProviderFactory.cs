using System;
using System.Collections.Generic;
using Intent.RelationalDbSchemaImporter.CLI.Providers.PostgreSQL;
using Intent.RelationalDbSchemaImporter.CLI.Providers.SqlServer;
using Intent.RelationalDbSchemaImporter.Contracts.Enums;

namespace Intent.RelationalDbSchemaImporter.CLI.Providers.Core;

/// <summary>
/// Factory implementation for creating database providers
/// </summary>
internal class DatabaseProviderFactory
{
    private readonly Dictionary<DatabaseType, Func<BaseDatabaseProvider>> _providerFactories;

    public DatabaseProviderFactory()
    {
        _providerFactories = new Dictionary<DatabaseType, Func<BaseDatabaseProvider>>
        {
            { DatabaseType.SqlServer, () => new SqlServerProvider() },
            { DatabaseType.PostgreSQL, () => new PostgreSQLProvider() }
        };
    }

    public BaseDatabaseProvider CreateProvider(DatabaseType databaseType)
    {
        if (databaseType == DatabaseType.Unspecified)
        {
            throw new ArgumentException("Cannot create provider for Auto database type.");
        }

        if (!_providerFactories.TryGetValue(databaseType, out var factory))
        {
            throw new NotSupportedException($"Database type {databaseType} is not supported.");
        }

        return factory();
    }

    public IEnumerable<DatabaseType> GetSupportedTypes()
    {
        return _providerFactories.Keys;
    }
} 