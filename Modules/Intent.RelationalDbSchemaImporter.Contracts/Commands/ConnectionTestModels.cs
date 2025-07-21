using Intent.RelationalDbSchemaImporter.Contracts.Enums;

namespace Intent.RelationalDbSchemaImporter.Contracts.Commands;

public class ConnectionTestRequest
{
    public string ConnectionString { get; set; } = string.Empty;
    public DatabaseType DatabaseType { get; set; }
}

public class ConnectionTestResult
{
    public bool IsSuccessful { get; set; }
}