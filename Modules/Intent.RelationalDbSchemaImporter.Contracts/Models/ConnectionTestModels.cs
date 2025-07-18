using Intent.RelationalDbSchemaImporter.Contracts.Enums;

namespace Intent.RelationalDbSchemaImporter.Contracts.Models;

public class ConnectionTestRequest
{
    public string ConnectionString { get; set; } = string.Empty;
    public DatabaseType DatabaseType { get; set; } = DatabaseType.Auto;
}

public class ConnectionTestResult
{
    public bool IsSuccessful { get; set; }
}