namespace Intent.Modules.Json.Importer.Tests.TestData;

/// <summary>
/// Object Mother factory for accessing test JSON file content.
/// Methods return the folder paths containing test JSON files.
/// </summary>
public static class JsonDocuments
{
    private static string GetTestFilesPath()
    {
        // Navigate from the test assembly location to the TestFiles folder
        var assemblyPath = Path.GetDirectoryName(typeof(JsonDocuments).Assembly.Location)!;
        return Path.GetFullPath(Path.Combine(assemblyPath, "..", "..", "..", "TestFiles"));
    }

    // Domain test file folders
    public static string DomainFolder() => Path.Combine(GetTestFilesPath(), "Domain");
    public static string DomainSubFolder() => Path.Combine(DomainFolder(), "SubFolder");

    // Eventing test file folders
    public static string EventingFolder() => Path.Combine(GetTestFilesPath(), "Eventing");

    // Services test file folders
    public static string ServicesFolder() => Path.Combine(GetTestFilesPath(), "Services");

    // Specific file paths for individual file tests
    public static string SimpleCustomerFile() => Path.Combine(DomainFolder(), "simple-customer.json");
    public static string InvoiceFile() => Path.Combine(DomainFolder(), "invoice.json");
    public static string CustomerWithExtraPropertyFile() => Path.Combine(DomainFolder(), "customer-with-extra-property.json");
    public static string CustomerWithMissingPropertyFile() => Path.Combine(DomainFolder(), "customer-with-missing-property.json");
    public static string ProductFile() => Path.Combine(DomainFolder(), "product.json");
    public static string AccountWithExtraPropertyFile() => Path.Combine(ServicesFolder(), "account-with-extra-property.json");
}
