namespace Intent.RelationalDbSchemaImporter.Contracts.DbSchema;

public class ComputedColumnSchema
{
    public string Expression { get; set; } = string.Empty;
    public bool IsPersisted { get; set; }
}
