using Intent.RelationalDbSchemaImporter.Contracts.Enums;

namespace Intent.RelationalDbSchemaImporter.Contracts.DbSchema;

/// <summary>
/// Represents a stored procedure in the database.
/// </summary>
public class StoredProcedureSchema
{
    /// <summary>
    /// The name of the stored procedure.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// The schema of the stored procedure.
    /// </summary>
    public string Schema { get; set; } = string.Empty;
    /// <summary>
    /// The parameters of the stored procedure.
    /// </summary>
    public List<StoredProcedureParameterSchema> Parameters { get; set; } = [];
    /// <summary>
    /// The result set columns of the stored procedure.
    /// </summary>
    public List<ResultSetColumnSchema> ResultSetColumns { get; set; } = [];
    /// <summary>
    /// Additional metadata about the stored procedure.
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// Represents a parameter of a stored procedure.
/// </summary>
public class StoredProcedureParameterSchema
{
    /// <summary>
    /// The name of the parameter.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// The data type of the parameter.
    /// </summary>
    public string DbDataType { get; set; } = string.Empty;
    /// <summary>
    /// Normalized to C# type representation (e.g., "string", "int", "datetime") for Intent type mapping.
    /// </summary>
    public string LanguageDataType { get; set; } = string.Empty;
    /// <summary>
    /// Parameter direction.
    /// </summary>
    public StoredProcedureParameterDirection Direction { get; set; } = StoredProcedureParameterDirection.In;
    /// <summary>
    /// The maximum length of the parameter.
    /// </summary>
    public int? MaxLength { get; set; }
    /// <summary>
    /// The numeric precision of the parameter.
    /// </summary>
    public int? NumericPrecision { get; set; }
    /// <summary>
    /// The numeric scale of the parameter.
    /// </summary>
    public int? NumericScale { get; set; }
    /// <summary>
    /// User-defined table type information if this parameter is a table-valued parameter.
    /// </summary>
    public UserDefinedTableTypeSchema? UserDefinedTableType { get; set; }
    /// <summary>
    /// Additional metadata about the parameter.
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// Represents a column in the result set of a stored procedure.
/// </summary>
public class ResultSetColumnSchema
{
    /// <summary>
    /// The name of the column.
    /// </summary>
    public string Name { get; set; } = null!;
    /// <summary>
    /// The data type of the column.
    /// </summary>
    public string DbDataType { get; set; } = string.Empty;

    public string? SourceSchema { get; set; }
    public string? SourceTable { get; set; }
    /// <summary>
    /// Normalized to C# type representation (e.g., "string", "int", "datetime") for Intent type mapping.
    /// </summary>
    public string LanguageDataType { get; set; } = string.Empty;
    /// <summary>
    /// Whether the column is nullable.
    /// </summary>
    public bool IsNullable { get; set; }
    /// <summary>
    /// The maximum length of the column.
    /// </summary>
    public int? MaxLength { get; set; }
    /// <summary>
    /// The numeric precision of the column.
    /// </summary>
    public int? NumericPrecision { get; set; }
    /// <summary>
    /// The numeric scale of the column.
    /// </summary>
    public int? NumericScale { get; set; }
    /// <summary>
    /// Additional metadata about the column.
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}
