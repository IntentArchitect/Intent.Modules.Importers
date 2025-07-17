namespace Intent.RelationalDbSchemaImporter.Contracts.Schema;

public class TriggerSchema
{
    public string Name { get; set; } = string.Empty;
    public string ParentSchema { get; set; } = string.Empty;
    public string ParentName { get; set; } = string.Empty;
    public string ParentType { get; set; } = string.Empty; // "Table" or "View"
    public Dictionary<string, object> Metadata { get; set; } = new();
}
