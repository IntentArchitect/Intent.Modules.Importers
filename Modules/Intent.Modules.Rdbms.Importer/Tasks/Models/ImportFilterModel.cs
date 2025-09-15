using Intent.RelationalDbSchemaImporter.Contracts.FileStructures;

namespace Intent.Modules.Rdbms.Importer.Tasks.Models;

public class FilterLoadInputModel
{
    public string ImportFilterFilePath { get; set; } = null!;
    public string PackageId { get; set; } = null!;
    public string ApplicationId { get; set; } = null!;
}

public class FilterSaveInputModel
{
    public string ImportFilterFilePath { get; set; } = null!;
    public string PackageId { get; set; } = null!;
    public string ApplicationId { get; set; } = null!;
    public ImportFilterSettings FilterData { get; set; } = null!;
}

public class PathResolutionInputModel
{
    public string SelectedFilePath { get; set; } = null!;
    public string PackageId { get; set; } = null!;
    public string ApplicationId { get; set; } = null!;
} 

public class VerifyFilePathInputModel
{
    // Path to the filter file to verify (may point to existing file or intended new file location)
    public string PathToFile { get; set; } = null!;
    public string PackageId { get; set; } = null!;
    public string ApplicationId { get; set; } = null!;
}