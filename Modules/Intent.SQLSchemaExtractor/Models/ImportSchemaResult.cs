namespace Intent.SQLSchemaExtractor.Models;

public class ImportSchemaResult
{
    public string PackageName { get; set; } = string.Empty;
    public string PackageFilePath { get; set; } = string.Empty;
    public int TablesImported { get; set; }
    public int ViewsImported { get; set; }
    public int StoredProceduresImported { get; set; }
    public int IndexesImported { get; set; }
}
