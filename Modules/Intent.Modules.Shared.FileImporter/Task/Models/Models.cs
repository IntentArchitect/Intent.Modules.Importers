namespace Intent.Modules.Json.Importer.Tasks.Models;

public class FilePreviewInputModel
{
    public string SourceFolder { get; set; } = string.Empty;
    public string Pattern { get; set; }
}

public record DirectoryPreview(string RootPath, string RootName, FilePreview[] Files);

public record FilePreview(string Name, string RelativePath, string FullPath);
