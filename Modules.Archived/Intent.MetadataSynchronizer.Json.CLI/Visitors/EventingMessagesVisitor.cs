using System.Text.Json;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.Modules.Common.Templates;

namespace Intent.MetadataSynchronizer.Json.CLI.Visitors;

internal sealed class EventingMessagesVisitor : IJsonElementVisitor
{
    public string DesignerName => "Services";

    public ElementPersistable VisitRoot(string name, string? parentFolderId, string externalReference) => ElementPersistable.Create(
        specializationType: "Message",
        specializationTypeId: "cbe970af-5bad-4d92-a3ed-a24b9fdaa23e",
        name: name,
        parentId: parentFolderId,
        externalReference: externalReference);

    public VisitorClassResult VisitObject(JsonConfig config, JsonProperty property, ElementPersistable owner, string? parentFolderId, string sourcePath, string targetPath, bool isCollection)
    {
        // Create an Eventing DTO and a property on the owner referencing it (no association)
        var compositeElement = ElementPersistable.Create(
            specializationType: "Eventing DTO",
            specializationTypeId: "544f1d57-27ce-4985-a4ec-cc01568d72b0",
            name: Utils.Casing(config, property.Name).Singularize(false),
            parentId: parentFolderId,
            externalReference: targetPath);

        var attrSpec = owner.SpecializationTypeId == "544f1d57-27ce-4985-a4ec-cc01568d72b0" ? "Eventing DTO-Field" : "Property";
        var attrSpecId = owner.SpecializationTypeId == "544f1d57-27ce-4985-a4ec-cc01568d72b0" ? "93eea5d7-a6a6-4fb8-9c87-d2e4c913fbe7" : "bde29850-5fb9-4f47-9941-b9e182fd9bdc";
        
        var ownerProperty = ElementPersistable.Create(
            specializationType: attrSpec,
            specializationTypeId: attrSpecId,
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
        var attrSpec = owner.SpecializationTypeId == "544f1d57-27ce-4985-a4ec-cc01568d72b0" ? "Eventing DTO-Field" : "Property";
        var attrSpecId = owner.SpecializationTypeId == "544f1d57-27ce-4985-a4ec-cc01568d72b0" ? "93eea5d7-a6a6-4fb8-9c87-d2e4c913fbe7" : "bde29850-5fb9-4f47-9941-b9e182fd9bdc";

        var attribute = ElementPersistable.Create(
            specializationType: attrSpec,
            specializationTypeId: attrSpecId,
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