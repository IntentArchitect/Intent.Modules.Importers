namespace Intent.RelationalDbSchemaImporter.Contracts.Schema;

public class ComputedColumnSchema
{
    public string Expression { get; set; } = string.Empty;
    public bool IsPersisted { get; set; }
}
