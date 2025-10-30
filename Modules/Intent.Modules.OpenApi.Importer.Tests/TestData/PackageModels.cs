using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Modules.Common.Types.Api;

namespace Intent.Modules.OpenApi.Importer.Tests.TestData;

/// <summary>
/// Object Mother factory for PackageModelPersistable instances.
/// Provides mock Intent metadata packages representing existing application state.
/// </summary>
public static class PackageModels
{
    public static PackageModelPersistable Empty()
    {
        return new PackageModelPersistable
        {
            Classes = []
        };
    }

    public static PackageModelPersistable WithTypeDefinitions()
    {
        var dictionary = ElementPersistable.Create(
            TypeDefinitionModel.SpecializationType, 
            TypeDefinitionModel.SpecializationTypeId, 
            "Dictionary", 
            null);
        dictionary.GenericTypes.AddRange([
            new GenericType { Name = "TKey" }, 
            new GenericType { Name = "TValue" }
        ]);

        return new PackageModelPersistable
        {
            Classes = new List<ElementPersistable>
            {
                ElementPersistable.Create(TypeDefinitionModel.SpecializationType, TypeDefinitionModel.SpecializationTypeId, "string", null),
                ElementPersistable.Create(TypeDefinitionModel.SpecializationType, TypeDefinitionModel.SpecializationTypeId, "int", null),
                ElementPersistable.Create(TypeDefinitionModel.SpecializationType, TypeDefinitionModel.SpecializationTypeId, "long", null),
                ElementPersistable.Create(TypeDefinitionModel.SpecializationType, TypeDefinitionModel.SpecializationTypeId, "decimal", null),
                ElementPersistable.Create(TypeDefinitionModel.SpecializationType, TypeDefinitionModel.SpecializationTypeId, "bool", null),
                ElementPersistable.Create(TypeDefinitionModel.SpecializationType, TypeDefinitionModel.SpecializationTypeId, "guid", null),
                ElementPersistable.Create(TypeDefinitionModel.SpecializationType, TypeDefinitionModel.SpecializationTypeId, "date", null),
                ElementPersistable.Create(TypeDefinitionModel.SpecializationType, TypeDefinitionModel.SpecializationTypeId, "datetime", null),
                ElementPersistable.Create(TypeDefinitionModel.SpecializationType, TypeDefinitionModel.SpecializationTypeId, "datetimeoffset", null),
                dictionary
            }
        };
    }

    public static PackageModelPersistable WithExistingService(string serviceName)
    {
        var package = WithTypeDefinitions();

        var service = ElementPersistable.Create("Service", Guid.NewGuid().ToString(), serviceName, null);
        package.Classes.Add(service);
        
        return package;
    }
}
