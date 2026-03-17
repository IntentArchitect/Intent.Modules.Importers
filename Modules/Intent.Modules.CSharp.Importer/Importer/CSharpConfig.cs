using System.ComponentModel.DataAnnotations;
using Intent.Persistence;

namespace Intent.Modules.CSharp.Importer.Importer;

public record ImportProfileConfig
{
    public required string Identifier { get; init; }
    public IElementSettings? MapInterfacesTo { get; init; }
    public IElementSettings? MapClassesTo { get; init; }
    public IElementSettings? MapPropertiesTo { get; init; }
    public IElementSettings? MapMethodsTo { get; init; }
    public IElementSettings? MapMethodParametersTo { get; init; }
    public IElementSettings? MapConstructorsTo { get; init; }
    public IElementSettings? MapConstructorParametersTo { get; init; }
    public IElementSettings? MapEnumsTo { get; init; }
    public IElementSettings? MapEnumLiteralsTo { get; init; }
    public IAssociationSettings? MapAssociationsTo { get; init; }
    public IAssociationSettings? MapInheritanceTo { get; init; }
    public ImportProfileConfig? DependencyProfile { get; set; }
    // Usage example: Service creation from Interface. If an interface implements another interface, should the parent interface methods be created on the child
    // or or their own element (or both)
    public ImportTypes MapBaseMethodsToChildTypes { get; init; }
    // Usage example: Service creation from Interface. If an interface implements another interface, should the parent interface result in their own element
    // or should it be skipped, usually used in conjunction with MapBaseMethodsToChildTypes
    public ImportTypes SkipBaseElementCreation { get; init; }
}

public record CSharpConfig
{
    public required ImportProfileConfig ImportProfile { get; init; }
    public string? TargetFolder { get; init; }
    public string? TargetFolderId { get; init; }
    public bool PreserveAsync { get; init; }

}

[Flags]
public enum ImportTypes
{
    Class = 1,
    Interface = 2
}