namespace Intent.RelationalDbSchemaImporter.Contracts.Schema;

public class ColumnSchema
{
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// Base data type name without size/precision information (e.g., "nvarchar", "decimal", "int")
    /// </summary>
    public string DataType { get; set; } = string.Empty;
    public bool IsNullable { get; set; }
    public bool IsPrimaryKey { get; set; }
    public bool IsIdentity { get; set; }
    /// <summary>
    /// Maximum length for string/binary types (e.g., 255 for nvarchar(255), -1 for max)
    /// </summary>
    public int? MaxLength { get; set; }
    /// <summary>
    /// Numeric precision for decimal/numeric types (e.g., 18 for decimal(18,2))
    /// </summary>
    public int? NumericPrecision { get; set; }
    /// <summary>
    /// Numeric scale for decimal/numeric types (e.g., 2 for decimal(18,2))
    /// </summary>
    public int? NumericScale { get; set; }
    public DefaultConstraintSchema? DefaultConstraint { get; set; }
    public ComputedColumnSchema? ComputedColumn { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}
