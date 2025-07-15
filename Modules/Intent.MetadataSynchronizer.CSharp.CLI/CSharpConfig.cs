using System.ComponentModel.DataAnnotations;

namespace Intent.MetadataSynchronizer.CSharp.CLI;

public record CSharpConfig
{
    public const string ConfigFile = nameof(ConfigFile);
    public const string GenerateConfigFile = nameof(GenerateConfigFile);

    // These Folder properties need to be updated in Program.cs and the README.md file
    // They are strongly tied to the CoreTypesData type too.
    public string? DomainEntitiesFolder { get; init; }
    public string? DomainEnumsFolder { get; init; }
    public string? DomainServicesFolder { get; init; }
    public string? DomainRepositoriesFolder { get; init; }
    public string? DomainDataContractsFolder { get; init; }
    public string? ServiceEnumsFolder { get; init; }
    public string? ServiceDtosFolder { get; init; }
    public string? ValueObjectsFolder { get; init; }
    public string? EventMessagesFolder { get; init; }
    public string IslnFile { get; init; } = null!;
    public string ApplicationName { get; init; } = null!;
    public string PackageId { get; init; } = null!;
    public string DesignerName { get; init; } = "Domain"; 
    public string? TargetFolderId { get; init; }
    public bool Debug { get; init; }

    public void Validate()
    {
        if (IslnFile is null)
        {
            throw new ValidationException($"{nameof(IslnFile)} is mandatory.");
        }
        
        if (ApplicationName is null)
        {
            throw new ValidationException($"{nameof(ApplicationName)} is mandatory.");
        }
        
        if (PackageId is null)
        {
            throw new ValidationException($"{nameof(PackageId)} is mandatory.");
        }

        if (DesignerName is null)
        {
            throw new ValidationException($"{nameof(DesignerName)} is mandatory.");
        }
    }
}