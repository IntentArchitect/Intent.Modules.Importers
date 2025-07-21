using System.Collections.Generic;
using System.Threading.Tasks;
using Intent.RelationalDbSchemaImporter.Contracts.DbSchema;
using Intent.RelationalDbSchemaImporter.Contracts.Enums;

namespace Intent.RelationalDbSchemaImporter.CLI.Providers;

/// <summary>
/// Defines the contract for database schema extraction providers
/// </summary>
public interface IDatabaseProvider
{
    /// <summary>
    /// The database type this provider supports
    /// </summary>
    DatabaseType SupportedType { get; }
    
    /// <summary>
    /// Extracts the complete database schema
    /// </summary>
    /// <param name="connectionString">Database connection string</param>
    /// <param name="config">Import configuration settings</param>
    /// <returns>Extracted database schema</returns>
    Task<DatabaseSchema> ExtractSchemaAsync(string connectionString, ImportConfiguration config);
    
    /// <summary>
    /// Tests database connectivity
    /// </summary>
    /// <param name="connectionString">Database connection string</param>
    /// <returns>True if connection is successful</returns>
    Task<bool> TestConnectionAsync(string connectionString);
}

/// <summary>
/// Handles dependency resolution between database objects
/// </summary>
public interface IDependencyResolver
{
    /// <summary>
    /// Gets all tables that depend on the specified tables (through foreign keys)
    /// </summary>
    /// <param name="tableNames">Source table names in format "schema.table"</param>
    /// <returns>Dependent table names</returns>
    Task<IEnumerable<string>> GetDependentTablesAsync(IEnumerable<string> tableNames);
}

/// <summary>
/// Analyzes stored procedures and functions to extract result set information
/// </summary>
public interface IStoredProcedureAnalyzer
{
    /// <summary>
    /// Analyzes a stored procedure to determine its result set columns
    /// </summary>
    /// <param name="procedureName">Name of the stored procedure</param>
    /// <param name="schema">Schema containing the procedure</param>
    /// <param name="parameters">Procedure parameters for analysis</param>
    /// <returns>Result set column information</returns>
    Task<List<ResultSetColumnSchema>> AnalyzeResultSetAsync(string procedureName, string schema, IEnumerable<StoredProcedureParameterSchema> parameters);
}

/// <summary>
/// Extension methods for database type conversions
/// </summary>
public static class DatabaseTypeExtensions
{
    /// <summary>
    /// Converts from contracts enum to provider enum
    /// </summary>
    public static DatabaseType ToProviderType(this Intent.RelationalDbSchemaImporter.Contracts.Enums.DatabaseType contractType)
    {
        return contractType switch
        {
            Intent.RelationalDbSchemaImporter.Contracts.Enums.DatabaseType.Auto => DatabaseType.Auto,
            Intent.RelationalDbSchemaImporter.Contracts.Enums.DatabaseType.SqlServer => DatabaseType.SqlServer,
            Intent.RelationalDbSchemaImporter.Contracts.Enums.DatabaseType.PostgreSQL => DatabaseType.PostgreSQL,
            Intent.RelationalDbSchemaImporter.Contracts.Enums.DatabaseType.MySQL => DatabaseType.MySQL,
            _ => DatabaseType.Auto
        };
    }
} 