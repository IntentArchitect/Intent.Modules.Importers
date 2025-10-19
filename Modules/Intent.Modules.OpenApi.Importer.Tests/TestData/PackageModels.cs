using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Modules.Common.Types.Api;

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
        
        // Add basic type definitions using the correct SpecializationType
        var types = new[]
        {
            "string",
            "int",
            "decimal",
            "bool",
            "guid",
            "datetime",
            "datetimeoffset",
            "date",
            "byte",
            "long",
            "double",
            "float",
            "object"
        };

        foreach (var typeName in types)
        {
            var type = ElementPersistable.Create(
                specializationType: TypeDefinitionModel.SpecializationType,
                specializationTypeId: TypeDefinitionModel.SpecializationTypeId,
                name: typeName,
                parentId: null);
            package.Classes.Add(type);
        }
        
        // Add Dictionary generic type for testing
        var dictionary = ElementPersistable.Create(
            TypeDefinitionModel.SpecializationType, 
            TypeDefinitionModel.SpecializationTypeId, 
            "Dictionary", 
            null);
        dictionary.GenericTypes.AddRange([
            new GenericType { Name = "TKey" }, 
            new GenericType { Name = "TValue" }
        ]);
        package.Classes.Add(dictionary);

        return package;
    }
}
