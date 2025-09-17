using Intent.Modelers.Domain.Api;

namespace Intent.MetadataSynchronizer.Json.CLI
{
    internal class JsonConfig
    {
        public JsonConfig()
        {
            SourceJsonFolder = null!;
            IslnFile = null!;
            ApplicationName = null!;
            PackageId = null!;
            CasingConvention = CasingConvention.AsIs;
            
            // Default to DomainDocumentDB for backwards compatibility
            Profile = ImportProfile.DomainDocumentDB;
        }

        public string SourceJsonFolder { get; set; }
        public string IslnFile { get; set; }
        public string ApplicationName { get; set; }
        public string PackageId { get; set; }
        public string? TargetFolderId { get; set; }
        public CasingConvention CasingConvention { get; set; }
        
        // Profile-based configuration
        public ImportProfile Profile { get; set; }

        #region Additional option names

        public static void ConfigFile() { }
        public static void GenerateConfigFile() { }

        #endregion
    }

    public enum CasingConvention
    {
        PascalCase,
        AsIs,
    }
}
