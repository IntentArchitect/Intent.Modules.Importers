using Intent.Modules.OpenApi.Importer.Importer;

namespace Intent.Modules.OpenApi.Importer.Tests.TestData;

internal static class ImportConfigurations
{
    public static ImportConfig CQRSWithPostfixes() => new()
    {
        ServiceType = ServiceType.CQRS,
        AddPostFixes = true,
        AllowRemoval = false,
        IsAzureFunctions = false,
        ReverseEngineerImplementation = false,
        IslnFile = "dummy.isln",
        ApplicationName = "TestApp",
        OpenApiSpecificationFile = "dummy.json",
        PackageId = "test-package-id"
    };

    public static ImportConfig CQRSWithoutPostfixes() => new()
    {
        ServiceType = ServiceType.CQRS,
        AddPostFixes = false,
        AllowRemoval = false,
        IslnFile = "dummy.isln",
        ApplicationName = "TestApp",
        OpenApiSpecificationFile = "dummy.json",
        PackageId = "test-package-id"
    };

    public static ImportConfig ServicePattern() => new()
    {
        ServiceType = ServiceType.Service,
        AddPostFixes = false,
        AllowRemoval = false,
        IslnFile = "dummy.isln",
        ApplicationName = "TestApp",
        OpenApiSpecificationFile = "dummy.json",
        PackageId = "test-package-id"
    };

    public static ImportConfig AzureFunctions() => new()
    {
        ServiceType = ServiceType.CQRS,
        IsAzureFunctions = true,
        AddPostFixes = true,
        IslnFile = "dummy.isln",
        ApplicationName = "TestApp",
        OpenApiSpecificationFile = "dummy.json",
        PackageId = "test-package-id"
    };

    public static ImportConfig WithDomainReverseEngineering() => new()
    {
        ServiceType = ServiceType.CQRS,
        ReverseEngineerImplementation = true,
        DomainPackageId = "test-domain-package-id",
        IslnFile = "dummy.isln",
        ApplicationName = "TestApp",
        OpenApiSpecificationFile = "dummy.json",
        PackageId = "test-package-id"
    };

    public static ImportConfig WithAllowRemoval() => new()
    {
        ServiceType = ServiceType.CQRS,
        AllowRemoval = true,
        IslnFile = "dummy.isln",
        ApplicationName = "TestApp",
        OpenApiSpecificationFile = "dummy.json",
        PackageId = "test-package-id"
    };
}
