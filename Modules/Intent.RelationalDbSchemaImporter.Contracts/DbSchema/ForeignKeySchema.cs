namespace Intent.RelationalDbSchemaImporter.Contracts.DbSchema;

public class ForeignKeySchema
{
    public string Name { get; set; } = string.Empty;
    public string ReferencedTableSchema { get; set; } = string.Empty;
    public string ReferencedTableName { get; set; } = string.Empty;
    public List<ForeignKeyColumnSchema> Columns { get; set; } = [];
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class ForeignKeyColumnSchema
{
    public string Name { get; set; } = string.Empty;
}
