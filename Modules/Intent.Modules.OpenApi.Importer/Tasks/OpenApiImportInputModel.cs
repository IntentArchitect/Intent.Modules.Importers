using Intent.Modules.OpenApi.Importer.Importer;

namespace Intent.Modules.OpenApi.Importer.Tasks
{
    public class OpenApiImportInputModel
    {
        public string OpenApiSpecificationFile { get; set; } = string.Empty;
        public string PackageId { get; set; } = string.Empty;
        public string? TargetFolderId { get; set; }
        public ServiceType ServiceType { get; set; } = ServiceType.CQRS;
        public bool AddPostFixes { get; set; } = true;
        public bool IsAzureFunctions { get; set; }
        public bool AllowRemoval { get; set; } = true;
        public SettingPersistence SettingPersistence { get; set; } = SettingPersistence.None;
        public bool ReverseEngineerImplementation { get; set; }
        public string? DomainPackageId { get; set; }
    }
}
