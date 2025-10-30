namespace Intent.Modules.OpenApi.Importer.Tasks;

public class ImportSettings
{
    public string OpenApiSpecificationFile { get; set; } = null!;
    public string? TargetFolderId { get; set; }
    public string PackageId { get; set; } = null!;

    public bool AddPostFixes { get; set; } = true;
    public bool AllowRemoval { get; set; } = true;
    public string ServiceType { get; set; } = "CQRS";
    public string? SettingPersistence { get; set; }
}
