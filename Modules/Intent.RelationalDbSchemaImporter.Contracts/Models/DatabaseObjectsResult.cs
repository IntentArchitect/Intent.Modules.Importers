namespace Intent.RelationalDbSchemaImporter.Contracts.Models;

public class DatabaseObjectsResult
{
    public List<string> Tables { get; set; } = [];
    public List<string> Views { get; set; } = [];
    public List<string> StoredProcedures { get; set; } = [];
}

public class StoredProceduresListResult
{
    public List<string> StoredProcedures { get; set; } = [];
}
