namespace Intent.RelationalDbSchemaImporter.Contracts.Schema;

public class ColumnSchema
{
    public string Name { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public bool IsNullable { get; set; }
    public bool IsPrimaryKey { get; set; }
    public bool IsIdentity { get; set; }
    public int? MaxLength { get; set; }
    public int? NumericPrecision { get; set; }
    public int? NumericScale { get; set; }
    public DefaultConstraintSchema? DefaultConstraint { get; set; }
    public ComputedColumnSchema? ComputedColumn { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}
