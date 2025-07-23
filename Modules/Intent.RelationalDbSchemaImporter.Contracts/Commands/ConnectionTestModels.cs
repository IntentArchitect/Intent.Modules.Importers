using Intent.RelationalDbSchemaImporter.Contracts.Enums;

namespace Intent.RelationalDbSchemaImporter.Contracts.Commands;

/// <summary>
/// Request model for testing a database connection.
/// </summary>
public class ConnectionTestRequest
{
    public string ConnectionString { get; set; } = string.Empty;
    public DatabaseType DatabaseType { get; set; }
}

/// <summary>
/// Response model for testing a database connection.
/// </summary>
public class ConnectionTestResult
{
    public bool IsSuccessful { get; set; }
}