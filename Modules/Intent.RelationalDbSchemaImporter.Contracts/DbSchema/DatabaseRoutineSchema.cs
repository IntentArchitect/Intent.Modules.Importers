namespace Intent.RelationalDbSchemaImporter.Contracts.DbSchema;

/// <summary>
/// Unified abstraction for database routines (stored procedures, functions, etc.)
/// </summary>
public class DatabaseRoutineSchema
{
    public string Name { get; set; } = string.Empty;
    public string Schema { get; set; } = string.Empty;
    public RoutineType RoutineType { get; set; }
    public List<RoutineParameterSchema> Parameters { get; set; } = [];
    public List<ResultSetColumnSchema> ResultSetColumns { get; set; } = [];
    /// <summary>
    /// Return type for scalar functions, null for procedures and table functions
    /// </summary>
    public string? ReturnType { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// Parameter for a database routine
/// </summary>
public class RoutineParameterSchema
{
    public string Name { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public ParameterDirection Direction { get; set; } = ParameterDirection.Input;
    public int? MaxLength { get; set; }
    public int? NumericPrecision { get; set; }
    public int? NumericScale { get; set; }
    public string? DefaultValue { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// Type of database routine
/// </summary>
public enum RoutineType
{
    /// <summary>
    /// SQL Server stored procedure
    /// </summary>
    StoredProcedure,
    
    /// <summary>
    /// Scalar function (returns single value) - SQL Server and PostgreSQL
    /// </summary>
    ScalarFunction,
    
    /// <summary>
    /// Table-valued function (returns table) - SQL Server and PostgreSQL
    /// </summary>
    TableFunction,
    
    /// <summary>
    /// PostgreSQL stored procedure (PostgreSQL 11+)
    /// </summary>
    Procedure,
    
    /// <summary>
    /// PostgreSQL aggregate function
    /// </summary>
    AggregateFunction,
    
    /// <summary>
    /// PostgreSQL window function
    /// </summary>
    WindowFunction
}

/// <summary>
/// Direction of a routine parameter
/// </summary>
public enum ParameterDirection
{
    Input,
    Output,
    InputOutput,
    ReturnValue
} 