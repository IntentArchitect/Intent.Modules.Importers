using Intent.Metadata.Models;
using Intent.Persistence;
using System.Collections.Generic;
using System.Xml.Linq;
using Intent.IArchitect.Agent.Persistence.Model;
using IElementPersistable = Intent.Persistence.IElementPersistable;
using Intent.MetadataSynchronizer;
using Intent.MetadataSynchronizer.CSharp.Importer;

namespace Intent.Modules.CSharp.Importer.Importer;

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

    private static readonly char[] Separators = { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };

    public IReadOnlyList<IElementPersistable> GetFolderElements(string relativeFolderPath, string? targetFolderId) => GetFolderElements(relativeFolderPath, targetFolderId != null ? _package.GetElementById(targetFolderId) : null);

    public IReadOnlyList<IElementPersistable> GetFolderElements(string relativeFolderPath, IElementPersistable? targetFolder)
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
                folderElement = (parentFolder?.ChildElements ?? _package.Classes).Add(
                    id: Guid.NewGuid().ToString().ToLower(),
                    specializationType: FolderModel.SpecializationType,
                    specializationTypeId: FolderModel.SpecializationTypeId,
                    name: curPart,
                    parentId: parentFolder?.Id ?? _package.Id,
                    externalReference: externalReference);
                _metadataLookup.AddElement(folderElement);
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


    public IAssociationPersistable? GetAssociationByReference(string externalReference)
    {
        return !_metadataLookup.TryGetAssociationByReference(externalReference, out var association)
            ? null
            : association;
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

    public IElementPersistable CreateElement(ClassData classData, string? targetFolderId)
    {
        var settings = _config.ImportProfile.MapClassesTo;
        if (settings == null)
        {
            throw new Exception("No Class settings specified for the import profile");
        }
        var externalReference = classData.GetIdentifier();
        if (GetElementByReference(externalReference) != null)
        {
            throw new Exception("An element with external reference \"" + externalReference + "\" already exists");
        }
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


        var element = _package.Classes.Add(
            id: Guid.NewGuid().ToString().ToLower(),
            specializationType: settings.SpecializationType,
            specializationTypeId: settings.SpecializationTypeId,
            name: classData.Name,
            parentId: folders.LastOrDefault()?.Id ?? targetFolderId ?? _package.Id,
            externalReference: externalReference);
        _metadataLookup.AddElement(element);
        _elementsToAdd.Add(element);
        return element;
    }

    public IElementPersistable? CreateElement(IElementSettings settings, string name, string relativeFilePath, string externalReference, string? targetFolderId)
    {
        if (settings == null)
        {
            throw new Exception("No Enum settings specified for the import profile");
        }

        if (GetElementByReference(externalReference) != null)
        {
            throw new Exception("An element with external reference \"" + externalReference + "\" already exists");
        }
        if (string.IsNullOrWhiteSpace(externalReference))
        {
            throw new ArgumentNullException(nameof(externalReference));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            ArgumentException.ThrowIfNullOrEmpty(nameof(name));
        }

        var folders = GetFolderElements(
            relativeFolderPath: GetRelativeLocation(relativeFilePath, _config.TargetFolder!),
            targetFolderId: _config.TargetFolderId);

        var element = _package.Classes.Add(
            id: Guid.NewGuid().ToString().ToLower(),
            specializationType: settings.SpecializationType,
            specializationTypeId: settings.SpecializationTypeId,
            name: name,
            parentId: folders.LastOrDefault()?.Id ?? targetFolderId ?? _package.Id,
            externalReference: externalReference);

        _metadataLookup.AddElement(element);
        _elementsToAdd.Add(element);
        return element;
    }

    public IAssociationPersistable CreateAssociation(string sourceElementId, string targetElementId)
    {
        var settings = _config.ImportProfile.MapAssociationsTo;
        if (settings == null)
        {
            throw new Exception($"Cannot create Association: {nameof(ImportProfileConfig.MapAssociationsTo)} was not specified.");
        }

        var association = _package.Associations.Add(settings, sourceElementId, targetElementId);
        _metadataLookup.AddAssociation(association);
        _associationsToAdd.Add(association);
        return association;
    }

    public void SetTypeReference(IElementPersistable targetElement, string type, bool isNullable, bool isCollection)
    {
        targetElement.TypeReference = new TypeReferencePersistable();
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

    public Persistables GetPersistables()
    {
        return new Persistables(_elementsToAdd, _associationsToAdd);
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