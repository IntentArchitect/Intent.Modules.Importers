using Intent.Modules.OpenApi.Importer.Importer;

namespace Intent.Modules.OpenApi.Importer.Tests.TestData;

/// <summary>
/// Object Mother factory for OpenApiImportConfig instances.
/// Provides consistent configuration objects for different test scenarios.
/// </summary>
public static class ImportConfigurations
{
    public static OpenApiImportConfig CQRSMode()
    {
        return new OpenApiImportConfig
        {
            ServiceType = ServiceType.CQRS,
            SettingPersistence = SettingPersistence.All
        };
    }

    public static OpenApiImportConfig ServiceMode()
    {
        return new OpenApiImportConfig
        {
            ServiceType = ServiceType.Service,
            SettingPersistence = SettingPersistence.All
        };
    }

    public static OpenApiImportConfig CQRSWithNoSettings()
    {
        return new OpenApiImportConfig
        {
            ServiceType = ServiceType.CQRS,
            SettingPersistence = SettingPersistence.None
        };
    }

    public static OpenApiImportConfig ServiceWithNoSettings()
    {
        return new OpenApiImportConfig
        {
            ServiceType = ServiceType.Service,
            SettingPersistence = SettingPersistence.None
        };
    }

    public static OpenApiImportConfig CQRSWithDeletions()
    {
        return new OpenApiImportConfig
        {
            ServiceType = ServiceType.CQRS,
            SettingPersistence = SettingPersistence.All,
            AllowRemoval = true
        };
    }

    public static OpenApiImportConfig ServiceWithDeletions()
    {
        return new OpenApiImportConfig
        {
            ServiceType = ServiceType.Service,
            SettingPersistence = SettingPersistence.All,
            AllowRemoval = true
        };
    }

    public static OpenApiImportConfig Custom(
        ServiceType serviceType,
        SettingPersistence settingPersistence,
        bool allowRemoval = false)
    {
        return new OpenApiImportConfig
        {
            ServiceType = serviceType,
            SettingPersistence = settingPersistence,
            AllowRemoval = allowRemoval
        };
    }
}
