using Intent.Modules.CSharp.Importer.Importer;
using Intent.Persistence;

namespace Intent.Modules.CSharp.Importer.Tests.TestData;

/// <summary>
/// Factory for creating import profile configurations for testing.
/// Provides pre-configured profiles without requiring a designer instance.
/// </summary>
public static class ImportConfigurations
{
    public static CSharpConfig DomainClassesProfile() => new()
    {
        TargetFolder = ".",  // Provide valid folder path for file organization logic
        ImportProfile = new ImportProfileConfig
        {
            Identifier = "domain-classes",
            MapClassesTo = new ElementSettings("04e12b51-ed12-42a3-9667-a6aa81bb6d10", "Class"),
            MapPropertiesTo = new ElementSettings("0090fb93-483e-41af-a11d-5ad2dc796adf", "Attribute"),
            MapInheritanceTo = new AssociationSettings("5de35973-3ac7-4e65-b48c-385605aec561", "Generalization",
                new AssociationEndSettings("ca8dabc4-a23c-4660-8859-fb4de53b72cd", "Source End"),
                new AssociationEndSettings("80f6663c-e0cb-48bd-b849-a421bba42e62", "Target End")),
            MapAssociationsTo = new AssociationSettings("eaf9ed4e-0b61-4ac1-ba88-09f912c12087", "Association",
                new AssociationEndSettings("3c12ff1f-d93c-438e-898d-1d87f0e78a5d", "Source End"),
                new AssociationEndSettings("c0b79cbd-38c4-41b5-9f9d-7da4ed4ccb82", "Target End")),
            MapEnumsTo = new ElementSettings("85fba0e9-9161-4c85-a603-a229ef312beb", "Enum"),
            MapEnumLiteralsTo = new ElementSettings("4215f417-25d2-4509-9309-5076a1452eaa", "Enum Literal"),
        }
    };

    public static CSharpConfig DomainEnumsProfile() => new()
    {
        TargetFolder = ".",  // Provide valid folder path for file organization logic
        ImportProfile = new ImportProfileConfig
        {
            Identifier = "domain-enums",
            MapEnumsTo = new ElementSettings("85fba0e9-9161-4c85-a603-a229ef312beb", "Enum"),
            MapEnumLiteralsTo = new ElementSettings("4215f417-25d2-4509-9309-5076a1452eaa", "Enum Literal"),
        }
    };

    public static CSharpConfig DomainInterfacesProfile() => new()
    {
        TargetFolder = ".",  // Provide valid folder path for file organization logic
        ImportProfile = new ImportProfileConfig
        {
            Identifier = "domain-interfaces",
            MapInterfacesTo = new ElementSettings("04e12b51-ed12-42a3-9667-a6aa81bb6d10", "Class"),
            MapMethodsTo = new ElementSettings("0caa00e7-38bc-4730-94e9-56cbddc6cae9", "Operation"),
            MapMethodParametersTo = new ElementSettings("590ba9f4-e5fd-4da1-a4e4-61040c2ae88f", "Parameter"),
        }
    };

    public static CSharpConfig TypeDefinitionsOnlyProfile() => new()
    {
        TargetFolder = ".",  // Provide valid folder path for file organization logic
        ImportProfile = new ImportProfileConfig
        {
            Identifier = "type-definitions-only",
            MapClassesTo = new ElementSettings("d4e577cd-ad05-4180-9a2e-fff4ddea0e1e", "Type-Definition"),
            MapInterfacesTo = new ElementSettings("d4e577cd-ad05-4180-9a2e-fff4ddea0e1e", "Type-Definition"),
        }
    };

    // Helper classes mirroring the internal ones from ImportCSharpFileInputModel
    private class ElementSettings(string specializationTypeId, string specializationType) : IElementSettings
    {
        public string SpecializationTypeId { get; } = specializationTypeId;
        public string SpecializationType { get; } = specializationType;
    }

    private class AssociationSettings(string specializationTypeId, string specializationType, IAssociationEndSetting sourceEnd, IAssociationEndSetting targetEnd) : IAssociationSettings
    {
        public string SpecializationTypeId { get; } = specializationTypeId;
        public string SpecializationType { get; } = specializationType;
        public IAssociationEndSetting SourceEnd { get; } = sourceEnd;
        public IAssociationEndSetting TargetEnd { get; } = targetEnd;
    }

    private class AssociationEndSettings(string specializationTypeId, string specializationType) : IAssociationEndSetting
    {
        public string SpecializationTypeId { get; } = specializationTypeId;
        public string SpecializationType { get; } = specializationType;
    }
}
