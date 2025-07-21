namespace Intent.RelationalDbSchemaImporter.Contracts.DbSchema;

public class IndexSchema
{
    public string Name { get; set; } = string.Empty;
    public bool IsUnique { get; set; }
    public bool IsClustered { get; set; }
    public bool HasFilter { get; set; }
    public string? FilterDefinition { get; set; }
    public List<IndexColumnSchema> Columns { get; set; } = [];
    public Dictionary<string, object> Metadata { get; set; } = new();
}
