namespace Intent.RelationalDbSchemaImporter.Contracts.Models;

public class ConnectionTestRequest
{
    public string ConnectionString { get; set; } = string.Empty;
}

public class ConnectionTestResult
{
    public bool IsSuccessful { get; set; }
}