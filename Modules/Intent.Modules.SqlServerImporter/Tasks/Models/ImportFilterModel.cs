using System.Collections.Generic;
using System.Text.Json.Serialization;
using Intent.RelationalDbSchemaImporter.Contracts.FileStructures;

namespace Intent.Modules.SqlServerImporter.Tasks.Models;

public class FilterLoadInputModel
{
    public string ImportFilterFilePath { get; set; } = null!;
    public string PackageId { get; set; }
    public string ApplicationId { get; set; }
}

public class FilterSaveInputModel
{
    public string ImportFilterFilePath { get; set; } = null!;
    public string PackageId { get; set; }
    public string ApplicationId { get; set; }
    public ImportFilterSettings FilterData { get; set; } = null!;
}

public class PathResolutionInputModel
{
    public string SelectedFilePath { get; set; } = null!;
    public string PackageId { get; set; }
    public string ApplicationId { get; set; }
} 