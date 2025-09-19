using System.Collections.Generic;
using Intent.MetadataSynchronizer.Json.CLI;

namespace Intent.Modules.Json.Importer.Tasks.Models;

public class JsonImportInputModel
{
    public string SourceFolder { get; set; } = string.Empty;
    public string PackageId { get; set; } = string.Empty;
    public string? TargetFolderId { get; set; }
    public ImportProfile Profile { get; set; }
    public List<string> SelectedFiles { get; set; } = new();
    public CasingConvention CasingConvention { get; set; }
}