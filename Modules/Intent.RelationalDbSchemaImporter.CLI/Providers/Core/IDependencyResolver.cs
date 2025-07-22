using System.Collections.Generic;
using System.Threading.Tasks;

namespace Intent.RelationalDbSchemaImporter.CLI.Providers.Core;

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