using System.Text.Json;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.Modules.Common.Templates;

namespace Intent.MetadataSynchronizer.Json.CLI.Visitors;

internal sealed class ServicesDtosVisitor : IJsonElementVisitor
{
    public string DesignerName => "Services";

    public ElementPersistable VisitRoot(string name, string? parentFolderId, string externalReference) => ElementPersistable.Create(
        specializationType: "DTO",
        specializationTypeId: "fee0edca-4aa0-4f77-a524-6bbd84e78734",
        name: name + "Dto",
        parentId: parentFolderId,
        externalReference: externalReference);

    public VisitorClassResult VisitObject(JsonConfig config, JsonProperty property, ElementPersistable owner, string? parentFolderId, string sourcePath, string targetPath, bool isCollection)
    {
        // Create a DTO and a field on the owner referencing it (no association)
        var compositeElement = ElementPersistable.Create(
            specializationType: "DTO",
            specializationTypeId: "fee0edca-4aa0-4f77-a524-6bbd84e78734",
            name: Utils.Casing(config, property.Name).Singularize(false) + "Dto",
            parentId: parentFolderId,
            externalReference: targetPath);

        var ownerProperty = ElementPersistable.Create(
            specializationType: "DTO-Field",
            specializationTypeId: "7baed1fd-469b-4980-8fd9-4cefb8331eb2",
            name: Utils.Casing(config, property.Name),
            parentId: owner.Id,
            externalReference: targetPath);

        ownerProperty.TypeReference = TypeReferencePersistable.Create(
            typeId: compositeElement.Id,
            isNavigable: true,
            isNullable: default,
            isCollection: isCollection,
            isRequired: default,
            comment: default,
            genericTypeId: default,
            typePackageName: default,
            typePackageId: default,
            stereotypes: [],
            genericTypeParameters: []);

        owner.ChildElements.Add(ownerProperty);
        return new VisitorClassResult(compositeElement, null);
    }

    public void VisitProperty(JsonConfig config, JsonProperty property, ElementPersistable owner, string externalReference, ElementPersistable referencedType, bool isCollection, string? remarks, bool isRootElement)
    {
        var attribute = ElementPersistable.Create(
            specializationType: "DTO-Field",
            specializationTypeId: "7baed1fd-469b-4980-8fd9-4cefb8331eb2",
            name: Utils.Casing(config, property.Name),
            parentId: owner.Id,
            externalReference: externalReference);
        
        attribute.TypeReference = TypeReferencePersistable.Create(
            typeId: referencedType.Id,
            isNavigable: true,
            isNullable: default,
            isCollection: isCollection,
            isRequired: default,
            comment: remarks,
            genericTypeId: default,
            typePackageName: referencedType.PackageName,
            typePackageId: referencedType.PackageId,
            stereotypes: [],
            genericTypeParameters: []);

        owner.ChildElements.Add(attribute);
    }
}
