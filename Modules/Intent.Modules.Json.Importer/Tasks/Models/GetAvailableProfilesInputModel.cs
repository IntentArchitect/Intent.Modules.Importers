namespace Intent.Modules.Json.Importer.Tasks.Models;

public class GetAvailableProfilesInputModel
{
    public string PackageId { get; set; } = string.Empty;
    public string? PackageSpecialization { get; set; }
}