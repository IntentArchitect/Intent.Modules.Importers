namespace Intent.RelationalDbSchemaImporter.Contracts.Schema;

public class IndexColumnSchema
{
    public string Name { get; set; } = string.Empty;
    public bool IsDescending { get; set; }
    public bool IsIncluded { get; set; }
}
