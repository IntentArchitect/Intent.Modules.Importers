using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.OpenApi.Importer.Importer
{
    public class ImportConfig
    {
        public ImportConfig()
        {
            ServiceType = ServiceType.CQRS;
            AddPostFixes = true;
            PackageId = null!;
            TargetFolderId = null!;
            IslnFile = null!;
            ApplicationName = null!;
            OpenApiSpecificationFile = null!;
            AllowRemoval = true;
        }

        public string IslnFile { get; set; }
        public string ApplicationName { get; set; }
        public string OpenApiSpecificationFile { get; set; }
        public string? TargetFolderId { get; set; }
        public string PackageId { get; set; }

        public bool AddPostFixes { get; set; }
        public bool IsAzureFunctions { get; set; }
        public ServiceType ServiceType { get; set; }
        public bool AllowRemoval { get; set; }
        public SettingPersistence SettingPersistence { get; set; } = SettingPersistence.None;

        public bool ReverseEngineerImplementation { get; set; }
        public string? DomainPackageId { get; set; }

        public static void ConfigFile() { }
        public static void GenerateConfigFile() { }
        public static void SerializedConfig() { }

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

}
