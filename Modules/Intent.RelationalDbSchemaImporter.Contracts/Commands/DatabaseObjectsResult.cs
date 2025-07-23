using Intent.RelationalDbSchemaImporter.Contracts.Enums;

namespace Intent.RelationalDbSchemaImporter.Contracts.Commands;

/// <summary>
/// Request model for retrieving database objects.
/// </summary>
public class DatabaseObjectsRequest
{
    public string ConnectionString { get; set; } = string.Empty;
    public DatabaseType DatabaseType { get; set; }
}

/// <summary>
/// Request model for retrieving stored procedures.
/// </summary>
public class StoredProceduresListRequest
{
    public string ConnectionString { get; set; } = string.Empty;
    public DatabaseType DatabaseType { get; set; }
}

/// <summary>
/// Response model for retrieving database objects.
/// </summary>
public class DatabaseObjectsResult
{
    public List<string> Tables { get; set; } = [];
    public List<string> Views { get; set; } = [];
    public List<string> StoredProcedures { get; set; } = [];
}

/// <summary>
/// Response model for retrieving stored procedures.
/// </summary>
public class StoredProceduresListResult
{
    public List<string> StoredProcedures { get; set; } = [];
}
