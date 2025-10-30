using System.Text.Json.Serialization;

namespace Intent.Modules.OpenApi.Importer.Importer;

public class OpenApiImportConfig
{
    public string IslnFile { get; set; } = null!;
    public string ApplicationName { get; set; } = null!;
    public string OpenApiSpecificationFile { get; set; } = null!;
    public string PackageId { get; set; } = null!;
    public string? TargetFolderId { get; set; }
    public bool AddPostFixes { get; set; } = true;
    public bool IsAzureFunctions { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ServiceType ServiceType { get; set; } = ServiceType.CQRS;
    public bool AllowRemoval { get; set; } = true;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SettingPersistence SettingPersistence { get; set; } = SettingPersistence.None;
}

public enum ServiceType
{
    CQRS,
    Service
}

public enum SettingPersistence
{
    None,
    All
}
