using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Modules.Common.Types.Api;

namespace Intent.Modules.Json.Importer.Tests.TestData;

/// <summary>
/// Object Mother factory for creating PackageModelPersistable instances.
/// </summary>
public static class PackageModels
{
    public static PackageModelPersistable EmptyDomainPackage() => CreatePackage(
        name: "TestDomain",
        specializationType: "Domain Package",
        specializationTypeId: "1a824508-4623-45d9-accc-f572091ade5a");

    public static PackageModelPersistable EmptyServicesPackage() => CreatePackage(
        name: "TestServices",
        specializationType: "Services Package",
        specializationTypeId: "df45eaf6-9202-4c03-90fb-dad1e2a45e4f");

    public static PackageModelPersistable WithDomainTypes()
    {
        var package = new PackageModelPersistable
        {
            Id = "test-package-id",
            Name = "TestDomain",
            SpecializationType = "Domain Package",
            SpecializationTypeId = "1a824508-4623-45d9-accc-f572091ade5a",
            Classes = new List<ElementPersistable>(),
            Associations = new List<AssociationPersistable>()
        };

        // Add type definitions as both Classes and to the types list
        var types = new List<ElementPersistable>
        {
            CreateTypeDefinition("string", package.Id, package.Name),
            CreateTypeDefinition("bool", package.Id, package.Name),
            CreateTypeDefinition("decimal", package.Id, package.Name),
            CreateTypeDefinition("guid", package.Id, package.Name),
            CreateTypeDefinition("datetime", package.Id, package.Name),
            CreateTypeDefinition("object", package.Id, package.Name)
        };

        foreach (var type in types)
        {
            package.Classes.Add(type);
        }

        return package;
    }

    public static PackageModelPersistable WithServicesTypes()
    {
        var package = new PackageModelPersistable
        {
            Id = "test-services-package-id",
            Name = "TestServices",
            SpecializationType = "Services Package",
            SpecializationTypeId = "df45eaf6-9202-4c03-90fb-dad1e2a45e4f",
            Classes = new List<ElementPersistable>(),
            Associations = new List<AssociationPersistable>()
        };

        // Add type definitions as both Classes and to the types list
        var types = new List<ElementPersistable>
        {
            CreateTypeDefinition("string", package.Id, package.Name),
            CreateTypeDefinition("bool", package.Id, package.Name),
            CreateTypeDefinition("decimal", package.Id, package.Name),
            CreateTypeDefinition("guid", package.Id, package.Name),
            CreateTypeDefinition("datetime", package.Id, package.Name),
            CreateTypeDefinition("object", package.Id, package.Name)
        };

        foreach (var type in types)
        {
            package.Classes.Add(type);
        }

        return package;
    }

    private static PackageModelPersistable CreatePackage(
        string name,
        string specializationType,
        string specializationTypeId)
    {
        return new PackageModelPersistable
        {
            Id = $"test-{name.ToLower()}-id",
            Name = name,
            SpecializationType = specializationType,
            SpecializationTypeId = specializationTypeId,
            Classes = new List<ElementPersistable>(),
            Associations = new List<AssociationPersistable>()
        };
    }

    private static ElementPersistable CreateTypeDefinition(string typeName, string packageId, string packageName)
    {
        var typeDefinition = ElementPersistable.Create(
            specializationType: TypeDefinitionModel.SpecializationType,
            specializationTypeId: TypeDefinitionModel.SpecializationTypeId,
            name: typeName,
            parentId: null,
            externalReference: null);

        typeDefinition.PackageId = packageId;
        typeDefinition.PackageName = packageName;

        return typeDefinition;
    }
}
