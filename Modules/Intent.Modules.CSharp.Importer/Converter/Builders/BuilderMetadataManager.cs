using Intent.Persistence;

namespace Intent.MetadataSynchronizer.CSharp.CLI.Builders;

public class BuilderMetadataManager
{
    private readonly MetadataLookup _metadataLookup;
    private readonly List<IElementPersistable> _elementsToAdd;
    private readonly IReadOnlyCollection<IPackageModelPersistable> _packages;

    public BuilderMetadataManager(MetadataLookup metadataLookup, List<IElementPersistable> elementsToAdd)
    {
        _metadataLookup = metadataLookup;
        _elementsToAdd = elementsToAdd;
    }

    public void SetTypeReferenceTypeId(IElementPersistable targetElement, string type)
    {
        if (_metadataLookup.TryGetTypeDefinitionByName(type, 0, out var typeDefElement))
        {
            targetElement.TypeReference.TypeId = typeDefElement.Id;
            targetElement.TypeReference.TypePackageId = typeDefElement.PackageId;
            targetElement.TypeReference.TypePackageName = typeDefElement.PackageName;
            return;
        }

        if (_metadataLookup.TryGetElementByReference(type, out var domainElement))
        {
            targetElement.TypeReference.TypeId = domainElement.Id;
            targetElement.TypeReference.TypePackageId = domainElement.PackageId;
            targetElement.TypeReference.TypePackageName = domainElement.PackageName;
            return;
        }
        
        //if (_metadataLookup.TryGetElementByReference(type, DomainServiceModel.SpecializationTypeId, out var domainServiceElement))
        //{
        //    targetElement.TypeReference.TypeId = domainServiceElement.Id;
        //    targetElement.TypeReference.TypePackageId = domainServiceElement.PackageId;
        //    targetElement.TypeReference.TypePackageName = domainServiceElement.PackageName;
        //    return;
        //}
        
        //if (_metadataLookup.TryGetElementByReference(type, EnumModel.SpecializationTypeId, out var enumElement))
        //{
        //    targetElement.TypeReference.TypeId = enumElement.Id;
        //    targetElement.TypeReference.TypePackageId = enumElement.PackageId;
        //    targetElement.TypeReference.TypePackageName = enumElement.PackageName;
        //    return;
        //}

        //if (_metadataLookup.TryGetElementByReference(type, ValueObjectModel.SpecializationTypeId, out var valueObjectElement))
        //{
        //    targetElement.TypeReference.TypeId = valueObjectElement.Id;
        //    targetElement.TypeReference.TypePackageId = valueObjectElement.PackageId;
        //    targetElement.TypeReference.TypePackageName = valueObjectElement.PackageName;
        //    return;
        //}

        //if (_metadataLookup.TryGetElementByReference(type, DataContractModel.SpecializationTypeId, out var dataContractElement))
        //{
        //    targetElement.TypeReference.TypeId = dataContractElement.Id;
        //    targetElement.TypeReference.TypePackageId = dataContractElement.PackageId;
        //    targetElement.TypeReference.TypePackageName = dataContractElement.PackageName;
        //    return;
        //}
        
        //if (_metadataLookup.TryGetElementByReference(type, RepositoryModel.SpecializationTypeId, out var repositoryElement))
        //{
        //    targetElement.TypeReference.TypeId = repositoryElement.Id;
        //    targetElement.TypeReference.TypePackageId = repositoryElement.PackageId;
        //    targetElement.TypeReference.TypePackageName = repositoryElement.PackageName;
        //    return;
        //}

        //if (_metadataLookup.TryGetElementByReference(type, MessageModel.SpecializationTypeId, out var eventMessageElement))
        //{
        //    targetElement.TypeReference.TypeId = eventMessageElement.Id;
        //    targetElement.TypeReference.TypePackageId = eventMessageElement.PackageId;
        //    targetElement.TypeReference.TypePackageName = eventMessageElement.PackageName;
        //    return;
        //}

        //if (_metadataLookup.TryGetElementByReference(type, EventingDTOModel.SpecializationTypeId, out var eventDtoElement))
        //{
        //    targetElement.TypeReference.TypeId = eventDtoElement.Id;
        //    targetElement.TypeReference.TypePackageId = eventDtoElement.PackageId;
        //    targetElement.TypeReference.TypePackageName = eventDtoElement.PackageName;
        //    return;
        //}

        //if (_metadataLookup.TryGetElementByReference(type, TypeDefinitionModel.SpecializationTypeId, out var typeDefCustomElement))
        //{
        //    targetElement.TypeReference.TypeId = typeDefCustomElement.Id;
        //    targetElement.TypeReference.TypePackageId = typeDefCustomElement.PackageId;
        //    targetElement.TypeReference.TypePackageName = typeDefCustomElement.PackageName;
        //    return;
        //}

        // Couldn't find an appropriate type. Let's create a Type-Def:
        var typeDefCustomElement = IElementPersistable.Create(
            specializationType: TypeDefinitionModel.SpecializationType,
            specializationTypeId: TypeDefinitionModel.SpecializationTypeId,
            name: type,
            parentId: null,
            externalReference: type);
        _metadataLookup.AddElement(typeDefCustomElement);
        _elementsToAdd.Add(typeDefCustomElement);

        targetElement.TypeReference.TypeId = typeDefCustomElement.Id;
        targetElement.TypeReference.TypePackageId = typeDefCustomElement.PackageId;
        targetElement.TypeReference.TypePackageName = typeDefCustomElement.PackageName;
    }
    
    public string? GetClassTypeId(string classReference)
    {
        return !_metadataLookup.TryGetElementByReference(classReference, out var domainElement) 
            ? null 
            : domainElement.Id;
    }
    
    public string? GetClassTypeName(string classReference)
    {
        return !_metadataLookup.TryGetElementByReference(classReference, out var domainElement) 
            ? null 
            : domainElement.Name;
    }
    
    public IElementPersistable? GetElementByReference(string externalReference, string typeId)
    {
        return !_metadataLookup.TryGetElementByReference(externalReference, out var element)
            ? null
            : element;
    }

    internal void AddElementForLookup(IElementPersistable element)
    {
        _metadataLookup.AddElementsIfMissing([element]);
    }
}