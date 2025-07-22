using System.Collections.Generic;
using System.Threading.Tasks;
using Intent.RelationalDbSchemaImporter.Contracts.DbSchema;

namespace Intent.RelationalDbSchemaImporter.CLI.Providers.Core;

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