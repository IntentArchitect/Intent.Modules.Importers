namespace Intent.MetadataSynchronizer.Json.CLI
{
    internal class JsonConfig
    {
        public JsonConfig()
        {
            SourceJsonFile = null!;
            IslnFile = null!;
            ApplicationName = null!;
            PackageId = null!;
            CasingConvention = CasingConvention.AsIs;
        }

        public string SourceJsonFile { get; set; }
        public string IslnFile { get; set; }
        public string ApplicationName { get; set; }
        public string PackageId { get; set; }
        public string? TargetFolderId { get; set; }
        public CasingConvention CasingConvention { get; set; }

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
