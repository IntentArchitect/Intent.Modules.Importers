using Intent.RelationalDbSchemaImporter.Contracts.Enums;

namespace Intent.RelationalDbSchemaImporter.Contracts.Commands;

public class DatabaseObjectsRequest
{
    public string ConnectionString { get; set; } = string.Empty;
    public DatabaseType DatabaseType { get; set; }
}

public class StoredProceduresListRequest
{
    public string ConnectionString { get; set; } = string.Empty;
    public DatabaseType DatabaseType { get; set; }
}

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
