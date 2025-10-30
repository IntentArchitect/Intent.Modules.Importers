using Intent.Modules.Json.Importer.Importer;

namespace Intent.Modules.Json.Importer.Tests.TestData;

/// <summary>
/// Object Mother factory for creating JsonConfig instances.
/// </summary>
public static class ImportConfigurations
{
    public static JsonConfig DomainProfile(string sourceFolder = "TestData", CasingConvention casing = CasingConvention.PascalCase) => new()
    {
        SourceJsonFolder = sourceFolder,
        IslnFile = "test.isln",
        ApplicationName = "TestApp",
        PackageId = "test-package-id",
        TargetFolderId = null,
        CasingConvention = casing,
        Profile = ImportProfile.DomainDocumentDB
    };

    public static JsonConfig EventingProfile(string sourceFolder = "TestData", CasingConvention casing = CasingConvention.PascalCase) => new()
    {
        SourceJsonFolder = sourceFolder,
        IslnFile = "test.isln",
        ApplicationName = "TestApp",
        PackageId = "test-services-package-id",
        TargetFolderId = null,
        CasingConvention = casing,
        Profile = ImportProfile.EventingMessages
    };

    public static JsonConfig ServicesProfile(string sourceFolder = "TestData", CasingConvention casing = CasingConvention.PascalCase) => new()
    {
        SourceJsonFolder = sourceFolder,
        IslnFile = "test.isln",
        ApplicationName = "TestApp",
        PackageId = "test-services-package-id",
        TargetFolderId = null,
        CasingConvention = casing,
        Profile = ImportProfile.ServicesDtos
    };

    public static JsonConfig WithTargetFolder(this JsonConfig config, string targetFolderId)
    {
        config.TargetFolderId = targetFolderId;
        return config;
    }

    public static JsonConfig WithCasing(this JsonConfig config, CasingConvention casing)
    {
        config.CasingConvention = casing;
        return config;
    }
}
