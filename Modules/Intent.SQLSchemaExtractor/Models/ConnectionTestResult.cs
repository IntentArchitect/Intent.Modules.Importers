namespace Intent.SQLSchemaExtractor.Models;

public class ConnectionTestResult
{
    public bool IsSuccessful { get; set; }
    public string DatabaseName { get; set; } = string.Empty;
    public string ServerName { get; set; } = string.Empty;
}
