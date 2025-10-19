using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;

namespace Intent.Modules.OpenApi.Importer.Tests.TestData;

internal static class PackageModels
{
    public static PackageModelPersistable Empty() => new()
    {
        Id = Guid.NewGuid().ToString(),
        Name = "TestServicesPackage",
        Classes = new List<ElementPersistable>(),
        Associations = new List<AssociationPersistable>()
    };

    public static PackageModelPersistable WithExistingCommand(string commandName)
    {
        var package = Empty();
        var command = ElementPersistable.Create(
            specializationType: "Command",
            specializationTypeId: "ccf14eb6-3a55-4d81-b5b9-d27311c70cb9",
            name: commandName,
            parentId: null,
            externalReference: $"Commands.{commandName}");
        package.Classes.Add(command);
        return package;
    }

    public static PackageModelPersistable WithExistingDTO(string dtoName)
    {
        var package = Empty();
        var dto = ElementPersistable.Create(
            specializationType: "DTO",
            specializationTypeId: "fee0edca-4aa0-4f77-a524-6bbd84e78734",
            name: dtoName,
            parentId: null,
            externalReference: $"DTOs.{dtoName}");
        package.Classes.Add(dto);
        return package;
    }

    public static PackageModelPersistable WithBasicTypes()
    {
        var package = Empty();
        var types = new[]
        {
            ("string", "d384db9c-a279-45e1-801e-e4e8099625f2"),
            ("int", "fb0a362d-e9e2-40de-b6a5-fcabc5484412"),
            ("decimal", "675c7b84-997e-40ff-a65e-fbabea28634a"),
            ("bool", "d28db340-56c5-4d94-a07e-b630e9fa4bea"),
            ("guid", "6b649125-18ea-48fd-a6ba-0bfff0d8f488"),
            ("datetime", "2611dd43-89e6-4cad-94ea-72cc38ee8ddf"),
            ("datetimeoffset", "2f2ac841-c6e9-47bd-8c6b-04fb5f7ba7bc")
        };

        foreach (var (name, typeId) in types)
        {
            var type = ElementPersistable.Create(
                specializationType: "Type-Definition",
                specializationTypeId: typeId,
                name: name,
                parentId: null);
            package.Classes.Add(type);
        }

        return package;
    }
}
