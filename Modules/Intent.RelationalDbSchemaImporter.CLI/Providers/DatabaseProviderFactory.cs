using System;
using System.Collections.Generic;
using Intent.RelationalDbSchemaImporter.CLI.Providers.PostgreSQL;
using Intent.RelationalDbSchemaImporter.CLI.Providers.SqlServer;

namespace Intent.RelationalDbSchemaImporter.CLI.Providers;

/// <summary>
/// Factory implementation for creating database providers
/// </summary>
public class DatabaseProviderFactory : IDatabaseProviderFactory
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
        if (databaseType == DatabaseType.Auto)
        {
            throw new ArgumentException("Cannot create provider for Auto database type. Use DetectDatabaseType first.");
        }

        if (!_providerFactories.ContainsKey(databaseType))
        {
            throw new NotSupportedException($"Database type {databaseType} is not supported.");
        }

        return _providerFactories[databaseType]();
    }

    public DatabaseType DetectDatabaseType(string connectionString)
    {
        try
        {
            // Convert to lowercase for easier pattern matching
            var connStr = connectionString.ToLowerInvariant();

            // SQL Server patterns
            if (connStr.Contains("server=") || connStr.Contains("data source=") || 
                connStr.Contains("initial catalog=") || connStr.Contains(".database.windows.net") ||
                connStr.Contains("sqlserver://") || connStr.Contains("mssql://") ||
                connStr.Contains("trust server certificate"))
            {
                return DatabaseType.SqlServer;
            }

            // PostgreSQL patterns
            if (connStr.Contains("host=") || connStr.Contains("postgresql://") || connStr.Contains("postgres://") ||
                connStr.Contains("port=5432") || connStr.Contains("username=") || connStr.Contains("user="))
            {
                return DatabaseType.PostgreSQL;
            }

            Logging.LogWarning("Could not auto-detect database type from connection string.");
            return DatabaseType.Auto;
        }
        catch (Exception ex)
        {
            Logging.LogWarning($"Error detecting database type: {ex.Message}");
            return DatabaseType.Auto;
        }
    }

    public IEnumerable<DatabaseType> GetSupportedTypes()
    {
        return _providerFactories.Keys;
    }
} 