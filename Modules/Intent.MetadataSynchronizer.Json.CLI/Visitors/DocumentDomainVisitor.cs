using System.Text.Json;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.Templates;

namespace Intent.MetadataSynchronizer.Json.CLI.Visitors;

internal sealed class DocumentDomainVisitor : IJsonElementVisitor
{
    public string DesignerName => "Domain";

    public ElementPersistable VisitRoot(string name, string? parentFolderId, string externalReference) => ElementPersistable.Create(
        specializationType: ClassModel.SpecializationType,
        specializationTypeId: ClassModel.SpecializationTypeId,
        name: name,
        parentId: parentFolderId,
        externalReference: externalReference);

    public VisitorClassResult VisitObject(JsonConfig config, JsonProperty property, ElementPersistable owner, string? parentFolderId, string sourcePath, string targetPath, bool isCollection)
    {
        var classElement = ElementPersistable.Create(
            specializationType: ClassModel.SpecializationType,
            specializationTypeId: ClassModel.SpecializationTypeId,
            name: Utils.Casing(config, property.Name).Singularize(false),
            parentId: parentFolderId,
            externalReference: targetPath);

        var association = new AssociationPersistable
        {
            Id = Guid.NewGuid().ToString().ToLower(),
            SourceEnd = new AssociationEndPersistable
            {
                SpecializationType = @"Association Source End",
                SpecializationTypeId = AssociationSourceEndModel.SpecializationTypeId,
                Name = owner.Name,
                TypeReference = TypeReferencePersistable.Create(
                    typeId: owner.Id,
                    isNavigable: false,
                    isNullable: false,
                    isCollection: false,
                    isRequired: default,
                    comment: default,
                    genericTypeId: default,
                    typePackageName: default,
                    typePackageId: default,
                    stereotypes: new List<StereotypePersistable>(),
                    genericTypeParameters: new List<TypeReferencePersistable>()),
                ExternalReference = sourcePath
            },
            TargetEnd = new AssociationEndPersistable
            {
                SpecializationType = @"Association Target End",
                SpecializationTypeId = AssociationTargetEndModel.SpecializationTypeId,
                Name = Utils.Casing(config, property.Name),
                TypeReference = TypeReferencePersistable.Create(
                    typeId: classElement.Id,
                    isNavigable: true,
                    isNullable: false,
                    isCollection: isCollection,
                    isRequired: default,
                    comment: default,
                    genericTypeId: default,
                    typePackageName: default,
                    typePackageId: default,
                    stereotypes: new List<StereotypePersistable>(),
                    genericTypeParameters: new List<TypeReferencePersistable>()),
                ExternalReference = targetPath
            },
            AssociationType = AssociationModel.SpecializationType,
            AssociationTypeId = AssociationModel.SpecializationTypeId
        };

        return new VisitorClassResult(classElement, association);
    }

    public void VisitProperty(JsonConfig config, JsonProperty property, ElementPersistable owner, string externalReference, ElementPersistable referencedType, bool isCollection, string? remarks, bool isRootElement)
    {
        var attribute = ElementPersistable.Create(
            specializationType: AttributeModel.SpecializationType,
            specializationTypeId: AttributeModel.SpecializationTypeId,
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

        // Add PK stereotype for Domain root Id
        if (isRootElement && string.Equals(attribute.Name, "Id", StringComparison.OrdinalIgnoreCase))
        {
            attribute.Stereotypes.Add(new StereotypePersistable
            {
                DefinitionId = "64f6a994-4909-4a9d-a0a9-afc5adf2ef74",
                Name = "Primary Key",
                Comment = null!,
                AddedByDefault = false,
                DefinitionPackageName = "Intent.Metadata.DocumentDB",
                DefinitionPackageId = "1c7eab18-9482-4b4e-b61b-1fbd2d2427b6",
                Properties = []
            });

            attribute.Metadata.Add(new GenericMetadataPersistable
            {
                Key = "is-managed-key",
                Value = "true"
            });
        }

        owner.ChildElements.Add(attribute);
    }
}