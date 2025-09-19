using Intent.Persistence;

namespace Intent.MetadataSynchronizer.CSharp.CLI.Builders;

public class AssociationBuilder
{
    private readonly BuilderMetadataManager _builderMetadataManager;
    private IAssociationPersistable _association;
    private bool _isBuilt;

    public AssociationBuilder(BuilderMetadataManager builderMetadataManager)
    {
        _builderMetadataManager = builderMetadataManager;
        var id = Guid.NewGuid().ToString().ToLower();

        _association = new AssociationPersistable
        {
            Id = id,
            AssociationType = AssociationModel.SpecializationType,
            AssociationTypeId = AssociationModel.SpecializationTypeId,
            SourceEnd = new AssociationEndPersistable
            {
                Id = Guid.NewGuid().ToString().ToLower(),
                SpecializationType = "Association Source End", // https://dev.azure.com/intentarchitect/Intent%20Architect/_workitems/edit/584
                SpecializationTypeId = AssociationSourceEndModel.SpecializationTypeId
            },
            TargetEnd = new AssociationEndPersistable
            {
                Id = id,
                SpecializationType = "Association Target End", // https://dev.azure.com/intentarchitect/Intent%20Architect/_workitems/edit/584
                SpecializationTypeId = AssociationTargetEndModel.SpecializationTypeId
            }
        };
    }

    public bool HasSource()
    {
        return _association.SourceEnd.TypeReference is not null;
    }

    public bool HasNavigableSource()
    {
        return _association.SourceEnd.TypeReference?.IsNavigable == true;
    }
    
    public AssociationBuilder AddBidirectionalSource(
        string sourceClassReference, 
        string bidirectionalFieldName, 
        bool bidirectionalIsNullable,
        bool bidirectionalIsCollection)
    {
        _association.SourceEnd.Name = bidirectionalFieldName;
        _association.SourceEnd.TypeReference.TypeId = _builderMetadataManager.GetClassTypeId(sourceClassReference);
        _association.SourceEnd.TypeReference.IsNavigable = true;
        _association.SourceEnd.TypeReference.IsNullable = bidirectionalIsNullable;
        _association.SourceEnd.TypeReference.IsCollection = bidirectionalIsCollection;
        _association.SourceEnd.TypeReference.TypeName = _association.SourceEnd.Name;
        return this;
    }
    
    public AssociationBuilder AddUnidirectionalSource(
        string sourceClassReference,
        string? fieldName,
        bool isNullable,
        bool isCollection)
    {
        _association.SourceEnd.Name = fieldName ?? _builderMetadataManager.GetClassTypeName(sourceClassReference);
        _association.SourceEnd.TypeReference.TypeId = _builderMetadataManager.GetClassTypeId(sourceClassReference);
        _association.SourceEnd.TypeReference.IsNavigable = false;
        _association.SourceEnd.TypeReference.IsNullable = isNullable;
        _association.SourceEnd.TypeReference.IsCollection = isCollection;
        _association.SourceEnd.TypeReference.TypeName = _association.SourceEnd.Name;
        return this;
    }
    
    // public AssociationBuilder AddUnidirectionalSource(string sourceClassReference, string typeId, string childReference)
    // {
    //     var classType = _builderMetadataManager.GetElementByReference(sourceClassReference, typeId);
    //     var child = classType.ChildElements.FirstOrDefault(p => p.ExternalReference == childReference);
    //     
    //     _association.SourceEnd.Name = child.Name;
    //     _association.SourceEnd.TypeReference = TypeReferencePersistable.Create(
    //         typeId: _builderMetadataManager.GetClassTypeId(sourceClassReference),
    //         isNavigable: false,
    //         isNullable: false,
    //         isCollection: false,
    //         isRequired: false,
    //         comment: null,
    //         genericTypeId: null,
    //         typePackageName: null,
    //         typePackageId: null,
    //         stereotypes: new List<StereotypePersistable>(),
    //         genericTypeParameters: new List<TypeReferencePersistable>());
    //     _association.SourceEnd.TypeReference.TypeName = _association.SourceEnd.Name;
    //     return this;
    // }
    
    public AssociationBuilder AddTarget(
        string targetClassReference, 
        string targetFieldName,
        bool targetIsNullable, 
        bool targetIsCollection)
    {
        _association.TargetEnd.Name = targetFieldName;
        _association.TargetEnd.TypeReference.TypeId = _builderMetadataManager.GetClassTypeId(targetClassReference);
        _association.TargetEnd.TypeReference.IsNavigable = true;
        _association.TargetEnd.TypeReference.IsNullable = targetIsNullable;
        _association.TargetEnd.TypeReference.IsCollection = targetIsCollection;
        _association.TargetEnd.TypeReference.TypeName = _association.TargetEnd.Name;
        return this;
    }
    
    public IAssociationPersistable Build()
    {
        if (_isBuilt)
        {
            throw new InvalidOperationException(
                $"The builder for {_association.ExternalReference} had already built the association. Make a new builder if you intended modify the association details.");
        }

        _isBuilt = true;
        return _association;
    }
}