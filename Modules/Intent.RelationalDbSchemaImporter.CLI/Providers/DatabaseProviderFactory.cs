using System;
using System.Collections.Generic;
using Intent.RelationalDbSchemaImporter.CLI.Providers.PostgreSQL;
using Intent.RelationalDbSchemaImporter.CLI.Providers.SqlServer;
using Intent.RelationalDbSchemaImporter.Contracts.Enums;

namespace Intent.RelationalDbSchemaImporter.CLI.Providers;

/// <summary>
/// Factory implementation for creating database providers
/// </summary>
internal class DatabaseProviderFactory : IDatabaseProviderFactory
{
    private readonly Dictionary<DatabaseType, Func<IDatabaseProvider>> _providerFactories;

    public DatabaseProviderFactory()
    {
        _providerFactories = new Dictionary<DatabaseType, Func<IDatabaseProvider>>
        {
            { DatabaseType.SqlServer, () => new SqlServerProvider() },
            { DatabaseType.PostgreSQL, () => new PostgreSQLProvider() }
        };
    }

    public IDatabaseProvider CreateProvider(DatabaseType databaseType)
    {
        if (databaseType == DatabaseType.Unspecified)
        {
            throw new ArgumentException("Cannot create provider for Auto database type.");
        }

        if (!_providerFactories.ContainsKey(databaseType))
        {
            throw new NotSupportedException($"Database type {databaseType} is not supported.");
        }

        return _providerFactories[databaseType]();
    }

    public IEnumerable<DatabaseType> GetSupportedTypes()
    {
        return _providerFactories.Keys;
    }
} 