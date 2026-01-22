using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.IArchitect.Agent.Persistence.Model.Module;
using Intent.IArchitect.Agent.Persistence.Serialization;
using Intent.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using IElementPersistable = Intent.Persistence.IElementPersistable;
using IInstalledModule = Intent.Persistence.IInstalledModule;

namespace Intent.MetadataSynchronizer;

public class MetadataLookup
{
    private const string TypeDefinitionId = "d4e577cd-ad05-4180-9a2e-fff4ddea0e1e";
    private Dictionary<(string Name, int TypeParamCount), IElementPersistable> _typeDefinitions;
    private Dictionary<string, IElementPersistable> _elementsByReference;
    private Dictionary<string, IElementPersistable[]> _elementsByName;
    private Dictionary<string, IElementPersistable> _elementsById;
    private Dictionary<string, IReadOnlyCollection<IElementPersistable>> _byParentId;
    /// <remarks>Contains each association twice, so that can be found by key either way.</remarks>
    private readonly Dictionary<string, IAssociationPersistable> _associationsByReference = new();
    private readonly Dictionary<string, HashSet<IAssociationPersistable>> _associationsByTypeId = new();

    public MetadataLookup(IPackageModelPersistable package)
    {
        var references = package.GetReferencedPackages();
        var packages = new[] { package }.Concat(references).ToList();
        Index(packages.SelectMany(x => x.GetAllElements()), packages.SelectMany(x => x.Associations));
    }

    public MetadataLookup(
        IReadOnlyCollection<IElementPersistable> elements,
        IReadOnlyCollection<IAssociationPersistable> associations)
    {
        Index(elements, associations);
    }

    private void Index(
        IEnumerable<IElementPersistable> elements,
        IEnumerable<IAssociationPersistable> associations)
    {
        // This grouping is used to identify elements that are exactly the same but coming from different package references,
        // We do a holistic comparison and if that matches we assume the same element.
        // If it doesn't match we can safely assume its a conflict between two different elements.
        var groupedElements = elements.GroupBy(e => new { e.Id, e.Display, e.Name, e.SpecializationTypeId })
            .Select(g => g.First());

        // Now do a count items grouped by Id - there should only be 1 item for each Id. If there are more,
        // then there are multiple different elements with the same Id
        var duplicateIds = groupedElements
            .GroupBy(g => g.Id)
            .Select(group => new { Id = group.Key, Count = group.Count(), Names = string.Join(",", group.Select(g => g.Display)) })
            .Where(g => g.Count > 1);

        if (duplicateIds.Any())
        {
            // build up a list of the duplicate Ids with the element names
            var errorString = string.Join(",", duplicateIds.Select(d => $"{d.Id} ({d.Names})"));
            throw new Exception($"Duplicate elements with the following Ids were found: {errorString}");
        }

        _elementsById = groupedElements.ToDictionary(d => d.Id);

        _typeDefinitions = _elementsById.Values
            .Where(x => x.SpecializationTypeId == TypeDefinitionId)
            .ToDictionary(x => (x.Name, x.GenericTypes.Count()));

        _elementsByName = _elementsById.Values
            .GroupBy(x => x.Name)
            .ToDictionary(x => x.Key, x => x.ToArray());

        _byParentId = _elementsById.Values
            .GroupBy(x => x.ParentFolderId)
            .ToDictionary(
                grouping => grouping.Key ?? string.Empty,
                grouping => (IReadOnlyCollection<IElementPersistable>)grouping.ToArray());

        // when building the _elementsByReference collection, only build the collection using the  "main" element types
        // which classes/interfaces/enums can be mapped to.
        // In theory we do not care about main parent items for duplicates, and then the children of those items
        // should have unique external references under that parent.
        var allowedTypes = new List<string>
        {
            { "04e12b51-ed12-42a3-9667-a6aa81bb6d10" }, // domain class
            { "85fba0e9-9161-4c85-a603-a229ef312beb" }, // enum
            { "0814e459-fb9b-47db-b7eb-32ce30397e8a" }, // domain event
            { "4464fabe-c59e-4d90-81fc-c9245bdd1afd" }, // domain contract
            { "b16578a5-27b1-4047-a8df-f0b783d706bd" }, // service
            { "ccf14eb6-3a55-4d81-b5b9-d27311c70cb9" }, // commands
            { "e71b0662-e29d-4db2-868b-8a12464b25d0" }, // queries
            { "fee0edca-4aa0-4f77-a524-6bbd84e78734" }, // dtos
            { "d4e577cd-ad05-4180-9a2e-fff4ddea0e1e" }, // type definition
            { "cbe970af-5bad-4d92-a3ed-a24b9fdaa23e" }, // integration message
            { "7f01ca8e-0e3c-4735-ae23-a45169f71625" }, // integration command
            { "544f1d57-27ce-4985-a4ec-cc01568d72b0" }, // event dto
            { "4d95d53a-8855-4f35-aa82-e312643f5c5f" }, // folders
        };

        _elementsByReference = _elementsById.Values
            .Where(x => !string.IsNullOrWhiteSpace(x.ExternalReference))
            .Where(x => allowedTypes.Contains(x.SpecializationTypeId))
            .ToDictionary(x => x.ExternalReference);

        foreach (var association in associations)
        {
            AddAssociation(association);
        }
    }

    public void ReIndex()
    {
        Index(_elementsById.Values, _associationsByTypeId.Values.SelectMany(x => x).Distinct().ToArray());
    }

    public IReadOnlyCollection<IElementPersistable> GetChildrenOf(string parentFolderId)
    {
        return _byParentId.TryGetValue(parentFolderId ?? string.Empty, out var items)
            ? items
            : Array.Empty<IElementPersistable>();
    }

    public void AddAssociation(IAssociationPersistable association)
    {
        if (association?.ExternalReference != null)
        {
            _associationsByReference.Add(association.ExternalReference, association);
        }

        if (!string.IsNullOrWhiteSpace(association.SourceEnd?.TypeReference?.TypeId))
        {
            if (!_associationsByTypeId.TryGetValue(association.SourceEnd.TypeReference.TypeId, out var associations))
            {
                associations = new HashSet<IAssociationPersistable>();
                _associationsByTypeId.Add(association.SourceEnd.TypeReference.TypeId, associations);
            }

            associations.Add(association);
        }

        if (!string.IsNullOrWhiteSpace(association.SourceEnd?.TypeReference?.TypeId) &&
            association.SourceEnd?.TypeReference.TypeId != association.TargetEnd.TypeReference.TypeId)
        {
            if (!_associationsByTypeId.TryGetValue(association.TargetEnd.TypeReference.TypeId, out var associations))
            {
                associations = new HashSet<IAssociationPersistable>();
                _associationsByTypeId.Add(association.TargetEnd.TypeReference.TypeId, associations);
            }

            associations.Add(association);
        }
    }

    public void AddElements(IEnumerable<IElementPersistable> elements)
    {
        foreach (var element in elements)
        {
            AddElement(element);
        }
    }

    /// <summary>
    /// Adds elements that are not already added and won't throw an exception if it is already added.
    /// </summary>
    /// <returns>For convenience, it will return a list of the elements that were added.</returns>
    public IReadOnlyCollection<IElementPersistable> AddElementsIfMissing(IReadOnlyCollection<IElementPersistable> elements)
    {
        var elementsAdded = new List<IElementPersistable>();
        foreach (var element in elements)
        {
            if (_elementsById.TryAdd(element.Id, element))
            {
                elementsAdded.Add(element);
            }
            _elementsByReference.TryAdd(element.ExternalReference, element);
        }
        return elementsAdded;
    }

    public void AddElement(IElementPersistable element)
    {
        _elementsById.Add(element.Id, element);
        _elementsByReference.Add(element.ExternalReference, element);
    }

    public IReadOnlyCollection<IAssociationPersistable> GetAssociationsFor(IElementPersistable element) =>
        GetAssociationsFor(element.Id);

    public IReadOnlyCollection<IAssociationPersistable> GetAssociationsFor(string elementId) =>
        _associationsByTypeId.TryGetValue(elementId, out var associations)
            ? associations
            : Array.Empty<IAssociationPersistable>();

    public bool TryGetElementById(string id, out IElementPersistable element) =>
        _elementsById.TryGetValue(id, out element);

    //public bool TryGetEnumByReference(string qualifiedName, out IElementPersistable element) =>
    //    TryGetElementByReference(qualifiedName, EnumModel.SpecializationTypeId, out element);

    //public bool TryGetClassByReference(string qualifiedName, out IElementPersistable element) =>
    //    TryGetElementByReference(qualifiedName, ClassModel.SpecializationTypeId, out element);

    public bool TryGetElementByReference(string reference, out IElementPersistable element)
    {
        if (_elementsByReference.TryGetValue(reference, out element))
        {
            return true;
        }

        element = default;
        return false;
    }

    public bool TryGetElementByName(string name, out IElementPersistable element)
    {
        if (_elementsByName.TryGetValue(name, out var elements) && elements.Length == 1)
        {
            element = elements[0];
            return true;
        }

        element = default;
        return false;
    }

    // Used specifically for folders (but can be used for other elements too)
    public bool TryGetElementByName(string name, string? parentId, out IElementPersistable element)
    {
        if (_elementsByName.TryGetValue(name, out var elements))
        {
            if(parentId != null)
            {
                element = elements.FirstOrDefault(e => e.ParentFolderId == parentId) ?? default;
                return true;
            }

            element = default;
            return false;
        }

        element = default;
        return false;
    }

    public bool TryGetTypeDefinitionByName(string name, int genericParameterCount, out IElementPersistable element) =>
        _typeDefinitions.TryGetValue((name, genericParameterCount), out element);

    public bool HasExistingAssociation(IAssociationPersistable association)
    {
        var a = _associationsByTypeId.TryGetValue(association.SourceEnd.TypeReference.TypeId, out var associationsFromSource);
        var b = _associationsByTypeId.TryGetValue(association.TargetEnd.TypeReference.TypeId, out var associationsFromTarget);
        if (!a && !b)
        {
            return false;
        }

        return associationsFromSource?.Any(ap => ap.SourceEnd.TypeReference.TypeId == association.SourceEnd.TypeReference.TypeId &&
                                                 ap.TargetEnd.TypeReference.TypeId == association.TargetEnd.TypeReference.TypeId &&
                                                 ap.TargetEnd.Name == association.TargetEnd.Name) == true
               ||
               associationsFromTarget?.Any(ap => ap.TargetEnd.TypeReference.TypeId == association.SourceEnd.TypeReference.TypeId &&
                                                 ap.SourceEnd.TypeReference.TypeId == association.TargetEnd.TypeReference.TypeId &&
                                                 ap.SourceEnd.Name == association.TargetEnd.Name) == true;
    }

    public bool TryGetAssociationByReference(string externalReference, out IAssociationPersistable? association)
    {
        return _associationsByReference.TryGetValue(externalReference, out association);
    }
}