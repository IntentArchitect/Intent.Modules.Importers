namespace Intent.RelationalDbSchemaImporter.Contracts.DbSchema;

public class StoredProcedureSchema
{
    public string Name { get; set; } = string.Empty;
    public string Schema { get; set; } = string.Empty;
    public List<StoredProcedureParameterSchema> Parameters { get; set; } = [];
    public List<ResultSetColumnSchema> ResultSetColumns { get; set; } = [];
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class StoredProcedureParameterSchema
{
    public string Name { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    /// <summary>
    /// Normalized data type for Intent type mapping (e.g., "string", "int", "datetime")
    /// </summary>
    public string NormalizedDataType { get; set; } = string.Empty;
    public bool IsOutputParameter { get; set; }
    public int? MaxLength { get; set; }
    public int? NumericPrecision { get; set; }
    public int? NumericScale { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class ResultSetColumnSchema
{
    public string Name { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    /// <summary>
    /// Normalized data type for Intent type mapping (e.g., "string", "int", "datetime")
    /// </summary>
    public string NormalizedDataType { get; set; } = string.Empty;
    public bool IsNullable { get; set; }
    public int? MaxLength { get; set; }
    public int? NumericPrecision { get; set; }
    public int? NumericScale { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}
