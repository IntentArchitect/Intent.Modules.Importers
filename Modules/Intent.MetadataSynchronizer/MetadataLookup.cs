using System;
using System.Collections.Generic;
using System.Linq;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.Types.Api;

namespace Intent.MetadataSynchronizer;

public class MetadataLookup
{
    private Dictionary<(string Name, int TypeParamCount), ElementPersistable> _typeDefinitions;
    private Dictionary<string, ElementPersistable> _elementsByReference;
    private Dictionary<string, ElementPersistable> _elementsById;
    private Dictionary<string, IReadOnlyCollection<ElementPersistable>> _byParentId;
    /// <remarks>Contains each association twice, so that can be found by key either way.</remarks>
    private readonly Dictionary<string, HashSet<AssociationPersistable>> _associationsByTypeId = new();


    public MetadataLookup(IReadOnlyCollection<PackageModelPersistable> packages)
    {
        Index(packages.SelectMany(x => x.GetAllElements()), packages.SelectMany(x => x.Associations));
    }

    public MetadataLookup(
        IReadOnlyCollection<ElementPersistable> elements,
        IReadOnlyCollection<AssociationPersistable> associations)
    {
        // Flatten elements to include all nested children for proper indexing by ExternalReference
        var allElements = FlattenElements(elements);
        Index(allElements, associations);
    }

    private static IEnumerable<ElementPersistable> FlattenElements(IEnumerable<ElementPersistable> elements)
    {
        foreach (var element in elements)
        {
            yield return element;

            foreach (var childElement in FlattenElements(element.ChildElements))
            {
                yield return childElement;
            }
        }
    }

    private void Index(
        IEnumerable<ElementPersistable> elements,
        IEnumerable<AssociationPersistable> associations)
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
            .Select(group => new { Id = group.Key, Count = group.Count(), Names = string.Join(",", group.Select(g => g.Display))})
            .Where(g => g.Count > 1);

        if (duplicateIds.Any())
        {
            // build up a list of the duplicate Ids with the element names
            var errorString = string.Join(",", duplicateIds.Select(d => $"{d.Id} ({d.Names})"));
            throw new Exception($"Duplicate elements with the following Ids were found: {errorString}");
        }

        _elementsById = groupedElements.ToDictionary(d => d.Id);

        _typeDefinitions = _elementsById.Values
            .Where(x => x.SpecializationTypeId == TypeDefinitionModel.SpecializationTypeId)
            .ToDictionary(x => (x.Name, x.GenericTypes.Count));

        _byParentId = _elementsById.Values
            .GroupBy(x => x.ParentFolderId)
            .ToDictionary(
                grouping => grouping.Key ?? string.Empty,
                grouping => (IReadOnlyCollection<ElementPersistable>)grouping.ToArray());

        var temp = _elementsById.Values
            .Where(x => !string.IsNullOrWhiteSpace(x.ExternalReference))
            .Select(x => new { x.SpecializationType, x.Name, x.ExternalReference, x.Id })
            .GroupBy(g => g.ExternalReference)
            .ToArray();
        
        _elementsByReference = _elementsById.Values
            .Where(x => !string.IsNullOrWhiteSpace(x.ExternalReference))
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

    public IReadOnlyCollection<ElementPersistable> GetChildrenOf(string parentFolderId)
    {
        return _byParentId.TryGetValue(parentFolderId ?? string.Empty, out var items)
            ? items
            : Array.Empty<ElementPersistable>();
    }

    public void AddAssociation(AssociationPersistable association)
    {
        if (!string.IsNullOrWhiteSpace(association.SourceEnd?.TypeReference?.TypeId))
        {
            if (!_associationsByTypeId.TryGetValue(association.SourceEnd.TypeReference.TypeId, out var associations))
            {
                associations = new HashSet<AssociationPersistable>();
                _associationsByTypeId.Add(association.SourceEnd.TypeReference.TypeId, associations);
            }

            associations.Add(association);
        }

        if (!string.IsNullOrWhiteSpace(association.SourceEnd?.TypeReference?.TypeId) &&
            association.SourceEnd?.TypeReference.TypeId != association.TargetEnd.TypeReference.TypeId)
        {
            if (!_associationsByTypeId.TryGetValue(association.TargetEnd.TypeReference.TypeId, out var associations))
            {
                associations = new HashSet<AssociationPersistable>();
                _associationsByTypeId.Add(association.TargetEnd.TypeReference.TypeId, associations);
            }

            associations.Add(association);
        }
    }

    public void AddElements(IEnumerable<ElementPersistable> elements)
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
    public IReadOnlyCollection<ElementPersistable> AddElementsIfMissing(IReadOnlyCollection<ElementPersistable> elements)
    {
        var elementsAdded = new List<ElementPersistable>();
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

    public void AddElement(ElementPersistable element)
    {
        _elementsById.Add(element.Id, element);
        _elementsByReference.Add(element.ExternalReference, element);
    }

    public IReadOnlyCollection<AssociationPersistable> GetAssociationsFor(ElementPersistable element) =>
        GetAssociationsFor(element.Id);

    public IReadOnlyCollection<AssociationPersistable> GetAssociationsFor(string elementId) =>
        _associationsByTypeId.TryGetValue(elementId, out var associations)
            ? associations
            : Array.Empty<AssociationPersistable>();

    public bool TryGetElementById(string id, out ElementPersistable element) =>
        _elementsById.TryGetValue(id, out element);

    public bool TryGetEnumByReference(string qualifiedName, out ElementPersistable element) =>
        TryGetElementByReference(qualifiedName, EnumModel.SpecializationTypeId, out element);

    public bool TryGetClassByReference(string qualifiedName, out ElementPersistable element) =>
        TryGetElementByReference(qualifiedName, ClassModel.SpecializationTypeId, out element);

    public bool TryGetElementByReference(string reference, string typeId, out ElementPersistable element)
    {
        if (_elementsByReference.TryGetValue(reference, out element) &&
            element.SpecializationTypeId == typeId)
        {
            return true;
        }

        element = default;
        return false;
    }

    public bool ContainsExternalReference(string externalReference)
    {
        return !string.IsNullOrEmpty(externalReference) && _elementsByReference.ContainsKey(externalReference);
    }

    public bool TryGetTypeDefinitionByName(string name, int genericParameterCount, out ElementPersistable element) =>
        _typeDefinitions.TryGetValue((name, genericParameterCount), out element);

    public bool HasExistingAssociation(AssociationPersistable association)
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
}