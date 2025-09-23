using System.ComponentModel.DataAnnotations;
using Intent.Persistence;

namespace Intent.Modules.CSharp.Importer.Importer;

public record ImportProfileConfig
{
    public required string Identifier { get; init; }
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
}

public record CSharpConfig
{
    public required ImportProfileConfig ImportProfile { get; init; }
    public string? TargetFolder { get; init; }
    public string? TargetFolderId { get; init; }
}