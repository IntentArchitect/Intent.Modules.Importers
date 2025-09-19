using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Eventing.Api;
using Intent.Modelers.Services.Api;
using ParameterModel = Intent.Modelers.Domain.Api.ParameterModel;

namespace Intent.MetadataSynchronizer.CSharp.CLI.Builders;

public record ParameterBuilder(string ParameterName, string ParameterType, bool ParameterTypeIsNullable, bool ParameterTypeIsCollection)
{
    public ElementPersistable Build()
    {
        var element = ElementPersistable.Create(
            specializationType: ParameterModel.SpecializationType,
            specializationTypeId: ParameterModel.SpecializationTypeId,
            name: ParameterName,
            parentId: null,
            externalReference: ParameterName);
        element.TypeReference = TypeReferencePersistable.Create(
            typeId: null,
            isNavigable: true,
            isNullable: ParameterTypeIsNullable,
            isCollection: ParameterTypeIsCollection,
            isRequired: false,
            comment: null,
            genericTypeId: null,
            typePackageName: null,
            typePackageId: null,
            stereotypes: new List<StereotypePersistable>(),
            genericTypeParameters: new List<TypeReferencePersistable>());

        return element;
    }
}

public record AttributeBuilder(string AttributeName, string AttributeType, bool AttributeTypeIsNullable, bool AttributeTypeIsCollection, string? Comment = null)
{
    public ElementPersistable Build()
    {
        var element = ElementPersistable.Create(
            specializationType: AttributeModel.SpecializationType,
            specializationTypeId: AttributeModel.SpecializationTypeId,
            name: AttributeName,
            parentId: null,
            externalReference: AttributeName);
        element.TypeReference = TypeReferencePersistable.Create(
            typeId: null,
            isNavigable: true,
            isNullable: AttributeTypeIsNullable,
            isCollection: AttributeTypeIsCollection,
            isRequired: false,
            comment: Comment,
            genericTypeId: null,
            typePackageName: null,
            typePackageId: null,
            stereotypes: new List<StereotypePersistable>(),
            genericTypeParameters: new List<TypeReferencePersistable>());
        element.Stereotypes = _stereotypes;
        return element;
    }

    private List<StereotypePersistable> _stereotypes = new List<StereotypePersistable>();

    public void AddStereotype(string stereotypeId, string stereotypeName, StereotypeProperty[] properties)
    {
        _stereotypes.Add(new StereotypePersistable
        {
            Name = stereotypeName,
            DefinitionId = stereotypeId,
            Properties = properties.Select(prop => new StereotypePropertyPersistable
            {
                DefinitionId = prop.Name,
                Value = prop.Value,
                Name = prop.Display,
                IsActive = true
            }).ToList()
        });
    }
}

public record PropertyBuilder(string PropertyName, string PropertyType, bool PropertyTypeIsNullable, bool PropertyTypeIsCollection, string? Comment = null)
{
    public ElementPersistable Build()
    {
        var element = ElementPersistable.Create(
            specializationType: PropertyModel.SpecializationType,
            specializationTypeId: PropertyModel.SpecializationTypeId,
            name: PropertyName,
            parentId: null,
            externalReference: PropertyName);
        element.TypeReference = TypeReferencePersistable.Create(
            typeId: null,
            isNavigable: true,
            isNullable: PropertyTypeIsNullable,
            isCollection: PropertyTypeIsCollection,
            isRequired: false,
            comment: Comment,
            genericTypeId: null,
            typePackageName: null,
            typePackageId: null,
            stereotypes: new List<StereotypePersistable>(),
            genericTypeParameters: new List<TypeReferencePersistable>());
        element.Stereotypes = _stereotypes;
        return element;
    }

    private List<StereotypePersistable> _stereotypes = new List<StereotypePersistable>();

    public void AddStereotype(string stereotypeId, string stereotypeName, StereotypeProperty[] properties)
    {
        _stereotypes.Add(new StereotypePersistable
        {
            Name = stereotypeName,
            DefinitionId = stereotypeId,
            Properties = properties.Select(prop => new StereotypePropertyPersistable
            {
                DefinitionId = prop.Name,
                Value = prop.Value,
                Name = prop.Display,
                IsActive = true
            }).ToList()
        });
    }
}

public record EventDtoFieldBuilder(string FieldName, string FieldType, bool FieldTypeIsNullable, bool FieldTypeIsCollection, string? Comment = null)
{
    public ElementPersistable Build()
    {
        var element = ElementPersistable.Create(
            specializationType: EventingDTOFieldModel.SpecializationType,
            specializationTypeId: EventingDTOFieldModel.SpecializationTypeId,
            name: FieldName,
            parentId: null,
            externalReference: FieldName);
        element.TypeReference = TypeReferencePersistable.Create(
            typeId: null,
            isNavigable: true,
            isNullable: FieldTypeIsNullable,
            isCollection: FieldTypeIsCollection,
            isRequired: false,
            comment: Comment,
            genericTypeId: null,
            typePackageName: null,
            typePackageId: null,
            stereotypes: new List<StereotypePersistable>(),
            genericTypeParameters: new List<TypeReferencePersistable>());
        element.Stereotypes = _stereotypes;
        return element;
    }

    private List<StereotypePersistable> _stereotypes = new List<StereotypePersistable>();

    public void AddStereotype(string stereotypeId, string stereotypeName, StereotypeProperty[] properties)
    {
        _stereotypes.Add(new StereotypePersistable
        {
            Name = stereotypeName,
            DefinitionId = stereotypeId,
            Properties = properties.Select(prop => new StereotypePropertyPersistable
            {
                DefinitionId = prop.Name,
                Value = prop.Value,
                Name = prop.Display,
                IsActive = true
            }).ToList()
        });
    }
}

public record DtoFieldBuilder(string FieldName, string FieldType, bool FieldTypeIsNullable, bool FieldTypeIsCollection, string? Comment = null)
{
    public ElementPersistable Build()
    {
        var element = ElementPersistable.Create(
            specializationType: DTOFieldModel.SpecializationType,
            specializationTypeId: DTOFieldModel.SpecializationTypeId,
            name: FieldName,
            parentId: null,
            externalReference: FieldName);
        element.TypeReference = TypeReferencePersistable.Create(
            typeId: null,
            isNavigable: true,
            isNullable: FieldTypeIsNullable,
            isCollection: FieldTypeIsCollection,
            isRequired: false,
            comment: Comment,
            genericTypeId: null,
            typePackageName: null,
            typePackageId: null,
            stereotypes: new List<StereotypePersistable>(),
            genericTypeParameters: new List<TypeReferencePersistable>());
        element.Stereotypes = _stereotypes;
        return element;
    }

    private List<StereotypePersistable> _stereotypes = new List<StereotypePersistable>();

    public void AddStereotype(string stereotypeId, string stereotypeName, StereotypeProperty[] properties)
    {
        _stereotypes.Add(new StereotypePersistable
        {
            Name = stereotypeName,
            DefinitionId = stereotypeId,
            Properties = properties.Select(prop => new StereotypePropertyPersistable
            {
                DefinitionId = prop.Name,
                Value = prop.Value,
                Name = prop.Display,
                IsActive = true
            }).ToList()
        });
    }
}

public record StereotypeProperty(string Name, string? Display, string? Value);