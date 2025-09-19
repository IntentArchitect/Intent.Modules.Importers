using Intent.IArchitect.Agent.Persistence.Model;
using Intent.Metadata.Models;
using Intent.Persistence;
using System.Collections.Generic;
using System.Xml.Linq;
using IElementPersistable = Intent.Persistence.IElementPersistable;

namespace Intent.MetadataSynchronizer.CSharp.CLI.Builders;

public class BuilderMetadataManager
{
    private readonly IPackageModelPersistable _package;
    private readonly CSharpConfig _config;
    private readonly MetadataLookup _metadataLookup;
    private readonly List<IElementPersistable> _elementsToAdd = [];
    private readonly List<IAssociationPersistable> _associationsToAdd = [];
    //private readonly IReadOnlyCollection<IPackageModelPersistable> _packages;

    public BuilderMetadataManager(IPackageModelPersistable package, CSharpConfig config)
    {
        _package = package;
        _config = config;
        _metadataLookup = new MetadataLookup(package);
    }

    //public BuilderMetadataManager(MetadataLookup metadataLookup, List<IElementPersistable> elementsToAdd)
    //{
    //    _metadataLookup = metadataLookup;
    //    _elementsToAdd = elementsToAdd;
    //}

    public void SetTypeReference(IElementPersistable targetElement, string type, bool isNullable, bool isCollection)
    {
        targetElement.TypeReference.IsNullable = isNullable;
        targetElement.TypeReference.IsCollection = isCollection;

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
        var typeDefCustomElement = _package.Classes.Add(
            id: Guid.NewGuid().ToString().ToLower(),
            specializationType: TypeDefinitionModel.SpecializationType,
            specializationTypeId: TypeDefinitionModel.SpecializationTypeId,
            name: type,
            parentId: _package.Id,
            externalReference: type);
        _metadataLookup.AddElement(typeDefCustomElement);
        _elementsToAdd.Add(typeDefCustomElement);

        targetElement.TypeReference.TypeId = typeDefCustomElement.Id;
        targetElement.TypeReference.TypePackageId = typeDefCustomElement.PackageId;
        targetElement.TypeReference.TypePackageName = typeDefCustomElement.PackageName;
    }


    private static readonly char[] Separators = { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };
    public IReadOnlyList<IElementPersistable> GetFolderElements(string relativeFolderPath, string targetFolderId) => GetFolderElements(relativeFolderPath, _package.GetElementById(targetFolderId));
    public IReadOnlyList<IElementPersistable> GetFolderElements(string relativeFolderPath, IElementPersistable targetFolder)
    {
        if (string.IsNullOrWhiteSpace(relativeFolderPath) || relativeFolderPath == ".")
        {
            return ArraySegment<IElementPersistable>.Empty;
        }

        var folders = new List<IElementPersistable>();
        var parentFolder = targetFolder;
        var parts = relativeFolderPath.Split(Separators, StringSplitOptions.RemoveEmptyEntries);
        for (var partIndex = 0; partIndex < parts.Length; partIndex++)
        {
            var curPart = parts[partIndex];
            if (curPart == ".")
            {
                continue;
            }

            var pathToHere = string.Join("-", parts.Take(partIndex + 1));
            var externalReference = $"Folder-{pathToHere}";

            if (!_metadataLookup.TryGetElementByReference(externalReference, out var folderElement))
            {
                folderElement = parentFolder.ChildElements.Add(
                    id: Guid.NewGuid().ToString().ToLower(),
                    specializationType: FolderModel.SpecializationType,
                    specializationTypeId: FolderModel.SpecializationTypeId,
                    name: curPart,
                    parentId: parentFolder.Id,
                    externalReference: externalReference);
                //folderElement = IElementPersistable.Create(
                //    specializationType: FolderModel.SpecializationType,
                //    specializationTypeId: FolderModel.SpecializationTypeId,
                //    name: curPart,
                //    parentId: parentFolderId,
                //    externalReference: externalReference);
            }

            parentFolder = folderElement;

            folders.Add(folderElement);
        }

        return folders;
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
    
    public IElementPersistable? GetElementByReference(string externalReference)
    {
        return !_metadataLookup.TryGetElementByReference(externalReference, out var element)
            ? null
            : element;
    }

    internal void AddElementForLookup(IElementPersistable element)
    {
        _metadataLookup.AddElementsIfMissing([element]);
    }

    public bool HasExistingAssociation(IAssociationPersistable association)
    {
        return _metadataLookup.HasExistingAssociation(association);
    }

    public bool TryGetElementById(string typeReferenceTypeId, out IElementPersistable element)
    {
        return _metadataLookup.TryGetElementById(typeReferenceTypeId, out element);
    }

    public IElementPersistable CreateElement(ClassData classData)
    {
        var externalReference = $"{classData.Namespace}.{classData.Name}";
        if (string.IsNullOrWhiteSpace(externalReference))
        {
            throw new ArgumentNullException(nameof(externalReference));
        }

        if (string.IsNullOrWhiteSpace(classData.Name))
        {
            ArgumentException.ThrowIfNullOrEmpty(nameof(classData.Name));
        }


        var folders = GetFolderElements(
            relativeFolderPath: GetRelativeLocation(classData.FilePath, _config.TargetFolder!),
            targetFolderId: _config.TargetFolderId);

        var settings = _package.GetDesigner().GetElementSettings(_config.ImportProfile.ClassToSpecializationTypeId);

        var element = _package.Classes.Add(
            id: Guid.NewGuid().ToString().ToLower(),
            specializationType: settings.SpecializationType,
            specializationTypeId: settings.SpecializationTypeId,
            name: classData.Name,
            parentId: folders.LastOrDefault()?.Id ?? _package.Id,
            externalReference: externalReference);
        _elementsToAdd.Add(element);
        return element;
    }

    private static string GetRelativeLocation(string? curFilePath, string targetFolder)
    {
        var curClassDir = Path.GetDirectoryName(curFilePath);
        if (string.IsNullOrWhiteSpace(curClassDir))
        {
            curClassDir = ".";
        }

        var newPath = Path.GetRelativePath(targetFolder, curClassDir);
        newPath = newPath.TrimStart('.', '\\', '/');
        return newPath;
    }

    public bool TryCreateAssociation(string sourceElementId, out IAssociationPersistable association)
    {
        association = null;
        var settings = _package.GetDesigner().GetAssociationSettings(_config.ImportProfile.AssociationSpecializationTypeId);
        if (settings == null)
        {
            return false;
        }

        association = _package.Associations.Add(settings, sourceElementId);
        _associationsToAdd.Add(association);
        return true;
    }

    public Persistables GetPersistables()
    {
        return new Persistables(_elementsToAdd, _associationsToAdd);
    }
}

public class FolderModel
{
    public const string SpecializationTypeId = "4d95d53a-8855-4f35-aa82-e312643f5c5f";
    public const string SpecializationType = "Folder";
}

public class TypeDefinitionModel
{
    public const string SpecializationTypeId = "d4e577cd-ad05-4180-9a2e-fff4ddea0e1e";
    public const string SpecializationType = "Type-Definition";
}