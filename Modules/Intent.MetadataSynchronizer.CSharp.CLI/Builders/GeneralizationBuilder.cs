using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Modelers.Domain.Api;

namespace Intent.MetadataSynchronizer.CSharp.CLI.Builders;

public class GeneralizationBuilder
{
    private readonly BuilderMetadataManager _builderMetadataManager;
    private AssociationPersistable _association;
    private bool _isBuilt;

    public GeneralizationBuilder(BuilderMetadataManager builderMetadataManager, string entityFullName, string baseTypeFullName)
    {
        _builderMetadataManager = builderMetadataManager;
        _association = new AssociationPersistable
        {
            Id = Guid.NewGuid().ToString().ToLower(),
            AssociationType = GeneralizationModel.SpecializationType,
            AssociationTypeId = GeneralizationModel.SpecializationTypeId,
            SourceEnd = new AssociationEndPersistable
            {
                Id = Guid.NewGuid().ToString().ToLower(),
                SpecializationType = "Generalization Source End",
                SpecializationTypeId = GeneralizationSourceEndModel.SpecializationTypeId,
                TypeReference = TypeReferencePersistable.Create(
                    typeId: _builderMetadataManager.GetClassTypeId(entityFullName),
                    isNavigable: false,
                    isNullable: false,
                    isCollection: false,
                    isRequired: true,
                    comment: null,
                    genericTypeId: null,
                    typePackageName: null,
                    typePackageId: null,
                    stereotypes: new List<StereotypePersistable>(),
                    genericTypeParameters: new List<TypeReferencePersistable>())
            },
            TargetEnd = new AssociationEndPersistable
            {
                Id = Guid.NewGuid().ToString().ToLower(),
                Name = "base",
                SpecializationType = "Generalization Target End",
                SpecializationTypeId = GeneralizationTargetEndModel.SpecializationTypeId,
                TypeReference = TypeReferencePersistable.Create(
                    typeId: _builderMetadataManager.GetClassTypeId(baseTypeFullName),
                    isNavigable: true,
                    isNullable: false,
                    isCollection: false,
                    isRequired: true,
                    comment: null,
                    genericTypeId: null,
                    typePackageName: null,
                    typePackageId: null,
                    stereotypes: new List<StereotypePersistable>(),
                    genericTypeParameters: new List<TypeReferencePersistable>())
            }
        };
        _association.SourceEnd.TypeReference.TypeName = _builderMetadataManager.GetClassTypeName(entityFullName);
        _association.TargetEnd.TypeReference.TypeName = _builderMetadataManager.GetClassTypeName(baseTypeFullName);
    }

    public AssociationPersistable Build()
    {
        if (_isBuilt)
        {
            throw new InvalidOperationException(
                $"The builder for {_association.ExternalReference} had already built the generalization. Make a new builder if you intended modify the generalization details.");
        }

        _isBuilt = true;
        return _association;
    }
}