namespace Intent.Modules.Json.Importer.Tasks.Models;

public class JsonPreviewInputModel
{
    public string SourceFolder { get; set; } = string.Empty;
    public string Pattern { get; set; } = "**/*.json";
}
