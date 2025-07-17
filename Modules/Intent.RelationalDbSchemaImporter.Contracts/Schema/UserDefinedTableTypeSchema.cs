namespace Intent.RelationalDbSchemaImporter.Contracts.Schema;

public class UserDefinedTableTypeSchema
{
    public string Name { get; set; } = string.Empty;
    public string Schema { get; set; } = string.Empty;
    public List<ColumnSchema> Columns { get; set; } = [];
    public Dictionary<string, object> Metadata { get; set; } = new();
}
