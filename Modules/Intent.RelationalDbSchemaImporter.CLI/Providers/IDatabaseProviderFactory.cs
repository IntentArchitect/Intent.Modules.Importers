using System.Collections.Generic;
using Intent.RelationalDbSchemaImporter.Contracts.Enums;

namespace Intent.RelationalDbSchemaImporter.CLI.Providers;

/// <summary>
/// Factory for creating database providers and detecting database types
/// </summary>
internal interface IDatabaseProviderFactory
{
    /// <summary>
    /// Creates a database provider for the specified database type
    /// </summary>
    /// <param name="databaseType">The type of database</param>
    /// <returns>Database provider instance</returns>
    IDatabaseProvider CreateProvider(DatabaseType databaseType);
    
    /// <summary>
    /// Gets all supported database types
    /// </summary>
    /// <returns>Collection of supported database types</returns>
    IEnumerable<DatabaseType> GetSupportedTypes();
} 