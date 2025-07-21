using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RelationalDbSchemaImporter.CLI.Services;
using Intent.RelationalDbSchemaImporter.Contracts.DbSchema;
using Intent.RelationalDbSchemaImporter.Contracts.Enums;
using Microsoft.Extensions.Logging;

namespace Intent.RelationalDbSchemaImporter.CLI.Providers;

/// <summary>
/// Defines the contract for database schema extraction providers
/// </summary>
internal interface IDatabaseProvider
{
    /// <summary>
    /// The database type this provider supports
    /// </summary>
    DatabaseType SupportedType { get; }
    
    /// <summary>
    /// Extracts the complete database schema
    /// </summary>
    /// <param name="connectionString">Database connection string</param>
    /// <param name="importFilterService">Import Filter Service</param>
    /// <returns>Extracted database schema</returns>
    Task<DatabaseSchema> ExtractSchemaAsync(string connectionString, ImportFilterService importFilterService, CancellationToken cancellationToken);
    
    /// <summary>
    /// Tests database connectivity
    /// </summary>
    /// <param name="connectionString">Database connection string</param>
    /// <remarks>It will throw an exception when the connection couldn't be established.</remarks>
    Task TestConnectionAsync(string connectionString, CancellationToken cancellationToken);
    
    /// <summary>
    /// Gets list of table names in the database
    /// </summary>
    /// <param name="connectionString">Database connection string</param>
    /// <returns>List of table names in format "schema.table"</returns>
    Task<List<string>> GetTableNamesAsync(string connectionString);
    
    /// <summary>
    /// Gets list of view names in the database
    /// </summary>
    /// <param name="connectionString">Database connection string</param>
    /// <returns>List of view names in format "schema.view"</returns>
    Task<List<string>> GetViewNamesAsync(string connectionString);
    
    /// <summary>
    /// Gets list of database routine names (stored procedures, functions, etc.)
    /// </summary>
    /// <param name="connectionString">Database connection string</param>
    /// <returns>List of routine names in format "schema.routine"</returns>
    Task<List<string>> GetRoutineNamesAsync(string connectionString);
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