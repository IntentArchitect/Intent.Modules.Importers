using System;
using System.Collections.Generic;
using System.Linq;
using Intent.MetadataSynchronizer.Configuration;
using Intent.Persistence;
using Intent.Utils;

namespace Intent.MetadataSynchronizer
{
    public class Synchronizer
    {
        public static void Execute(
            IPackageModelPersistable targetPackage,
            string parentFolderId,
            Persistables persistables,
            bool deleteExtra,
            bool createAttributesWithUnknownTypes,
            StereotypeManagementMode stereotypeManagementMode)
        {
            var incomingLookup = new MetadataLookup(persistables.Elements, persistables.Associations);
            var packageLookup = new MetadataLookup(targetPackage);
            var idMap = new Dictionary<string, string>();
            var matchedIds = new HashSet<string>();

            if (parentFolderId != targetPackage.Id &&
                targetPackage.GetElementById(parentFolderId) == null)
            {
                throw new Exception($"Could not find element with Id {parentFolderId}");
            }

            Logging.Log.Info(Indentation.Get() + "Processing elements");
            using (new Indentation())
            {
                SynchronizeElements(
                    incomingParentId: null,
                    packageParentId: parentFolderId,
                    incomingLookup: incomingLookup,
                    packageLookup: packageLookup,
                    idMap: idMap,
                    matchedIds: matchedIds,
                    createAttributesWithUnknownTypes: createAttributesWithUnknownTypes,
                    stereotypeManagementMode: stereotypeManagementMode,
                    addToParent: element =>
                    {
                        if (packageLookup.TryGetElementById(element.Id, out _))
                        {
                            return;
                        }

                        packageLookup.AddElement(element);

                        if (element.ParentFolderId == null ||
                            element.ParentFolderId == targetPackage.Id)
                        {
                            targetPackage.AddElement(element);
                            return;
                        }

                        if (!packageLookup.TryGetElementById(idMap[element.ParentFolderId], out var parentElement))
                        {
                            throw new Exception("Unable to locate parent element.");
                        }

                        parentElement.ChildElements.Add(element);
                    });
            }

            Logging.Log.Info(Indentation.Get() + "Updating IDs and type references");
            using (new Indentation())
            {
                RemapTypeReferences(
                    incomingElements: persistables.Elements,
                    incomingAssociations: persistables.Associations,
                    idMap: idMap);

                incomingLookup.ReIndex();
                packageLookup.ReIndex();
            }

            Logging.Log.Info(Indentation.Get() + "Processing associations");
            using (new Indentation())
            {
                SynchronizeAssociations(
                    packageLookup: packageLookup,
                    incomingAssociations: persistables.Associations,
                    matchedIds: matchedIds,
                    addToPackage: association =>
                    {
                        if (targetPackage.Associations.Any(x => x.Id == association.Id))
                        {
                            return;
                        }

                        // Sets Ids as per convention. Prevents Ids changing after re-running importer again
                        // or saving in the designer.
                        //association.SourceEnd.Id = null;
                        //association.TargetEnd.Id = null;
                        //association.Load();

                        targetPackage.Associations.Add(association);
                    });
            }

            if (deleteExtra)
            {
                Logging.Log.Info(Indentation.Get() + "Deleting extra elements and associations from the package.");
                using (new Indentation())
                {
                    DeleteExtra(
                        matchedIds: matchedIds,
                        package: targetPackage);
                }
            }
        }

        private static void DeleteExtra(
            IReadOnlySet<string> matchedIds,
            IPackageModelPersistable package)
        {
            RemoveElements(package.Classes);

            foreach (var association in package.Associations.ToArray())
            {
                if (!AssociationHasExternalReference(association) ||
                    matchedIds.Contains(association.Id))
                {
                    continue;
                }

                Logging.Log.Debug(Indentation.Get() + "Removing association \"{Association}\" ({Id})", association, association.Id);
                package.Associations.Remove(association);
            }

            void RemoveElements(ICollection<IElementPersistable> elements)
            {
                foreach (var element in elements)
                {
                    RemoveElements(element.ChildElements);
                }

                foreach (var element in elements.ToArray())
                {
                    if (!ElementHasExternalReference(element) ||
                        matchedIds.Contains(element.Id))
                    {
                        continue;
                    }

                    Logging.Log.Debug(Indentation.Get() + "Removing element \"{Name}\" ({Id}) with {ExternalReference}", element.Name, element.Id, element.ExternalReference);
                    elements.Remove(element);
                }
            }

            static bool ElementHasExternalReference(IElementPersistable element) =>
                !string.IsNullOrWhiteSpace(element.ExternalReference);

            static bool AssociationHasExternalReference(IAssociationPersistable association) =>
                !string.IsNullOrWhiteSpace(association.ExternalReference) ||
                !string.IsNullOrWhiteSpace(association.SourceEnd?.ExternalReference) ||
                !string.IsNullOrWhiteSpace(association.TargetEnd?.ExternalReference);
        }

        private static void SynchronizeAssociations(
            MetadataLookup packageLookup,
            IEnumerable<IAssociationPersistable> incomingAssociations,
            ISet<string> matchedIds,
            Action<IAssociationPersistable> addToPackage)
        {
            foreach (var association in incomingAssociations)
            {
                var matchedByType = Enumerable.Empty<IAssociationPersistable>()
                    .Concat(packageLookup.GetAssociationsFor(GetElementForSourceEnd(association)))
                    .Concat(packageLookup.GetAssociationsFor(GetElementForTargetEnd(association)))
                    .Distinct()
                    .Where(item => !matchedIds.Contains(item.Id) &&
                                   MatchesOnEndTypes(item, association))
                    .ToArray();

                var packageAssociation =
                    matchedByType.SingleOrDefault(item => MatchesOnEndExternalReferences(item, association)) ??
                    matchedByType.FirstOrDefault(item => MatchesOnEndNames(item, association)) ??
                    matchedByType.FirstOrDefault() ??
                    association;

                matchedIds.Add(packageAssociation.Id);
                addToPackage(packageAssociation);
            }

            static bool MatchesOnEndTypes(IAssociationPersistable @this, IAssociationPersistable other)
            {
                if (@this.SourceEnd?.TypeReference?.TypeId == other.SourceEnd?.TypeReference?.TypeId &&
                    @this.TargetEnd?.TypeReference?.TypeId == other.TargetEnd?.TypeReference?.TypeId)
                {
                    return true;
                }

                if (@this.SourceEnd?.TypeReference?.TypeId == other.TargetEnd?.TypeReference?.TypeId &&
                    @this.TargetEnd?.TypeReference?.TypeId == other.SourceEnd?.TypeReference?.TypeId)
                {
                    return true;
                }

                return false;
            }

            static bool MatchesOnEndExternalReferences(IAssociationPersistable @this, IAssociationPersistable other)
            {
                if (@this.SourceEnd?.ExternalReference == other.SourceEnd?.ExternalReference &&
                    @this.TargetEnd?.ExternalReference == other.TargetEnd?.ExternalReference)
                {
                    return true;
                }

                if (@this.SourceEnd?.ExternalReference == other.TargetEnd?.ExternalReference &&
                    @this.TargetEnd?.ExternalReference == other.SourceEnd?.ExternalReference)
                {
                    return true;
                }

                return false;
            }

            static bool MatchesOnEndNames(IAssociationPersistable @this, IAssociationPersistable other)
            {
                if (@this.SourceEnd?.Name == other.SourceEnd?.Name &&
                    @this.TargetEnd?.Name == other.TargetEnd?.Name)
                {
                    return true;
                }

                if (@this.SourceEnd?.Name == other.TargetEnd?.Name &&
                    @this.TargetEnd?.Name == other.SourceEnd?.Name)
                {
                    return true;
                }

                return false;
            }

            IElementPersistable GetElementForSourceEnd(IAssociationPersistable association)
            {
                var element = GetElementFor(association.SourceEnd);
                if (element is null)
                {
                    throw new Exception($"Association [{association.SourceEnd.Name} -> {association.TargetEnd.Name}] could not resolve Element for Source End. TypeId: {association.SourceEnd.TypeReference?.TypeId ?? "Not specified"}");
                }

                return element;
            }
            
            IElementPersistable GetElementForTargetEnd(IAssociationPersistable association)
            {
                var element = GetElementFor(association.TargetEnd);
                if (element is null)
                {
                    throw new Exception($"Association [{association.SourceEnd.Name} -> {association.TargetEnd.Name}] could not resolve Element for Target End. TypeId: {association.TargetEnd.TypeReference?.TypeId ?? "Not specified"}");
                }

                return element;
            }
            
            IElementPersistable GetElementFor(IAssociationEndPersistable associationEnd)
            {
                if (associationEnd?.TypeReference?.TypeId != null &&
                    packageLookup.TryGetElementById(associationEnd.TypeReference.TypeId, out var element))
                {
                    return element;
                }

                return null;
            }
        }

        private static void SynchronizeElements(string incomingParentId,
            string packageParentId,
            MetadataLookup incomingLookup,
            MetadataLookup packageLookup,
            IDictionary<string, string> idMap,
            ISet<string> matchedIds,
            bool createAttributesWithUnknownTypes,
            StereotypeManagementMode stereotypeManagementMode,
            Action<IElementPersistable> addToParent)
        {
            var incoming = incomingLookup.GetChildrenOf(incomingParentId);
            var candidates = packageLookup.GetChildrenOf(packageParentId);

            foreach (var incomingElement in incoming)
            {
                var element = ResolveElement(incomingElement);
                if (element.SpecializationTypeId == AttributeModel.SpecializationTypeId &&
                    element.TypeReference == null &&
                    !createAttributesWithUnknownTypes)
                {
                    continue;
                }

                element.Name = incomingElement.Name;
                element.ParentFolderId = packageParentId;

                matchedIds.Add(element.Id);
                idMap.Add(incomingElement.Id, element.Id);

                if (incomingElement.TypeReference?.TypeId == null &&
                    element.TypeReference?.TypeId != null)
                {
                    element.TypeReference = incomingElement.TypeReference;
                }

                SynchronizeTypeReference(
                    incoming: incomingElement.TypeReference,
                    package: element.TypeReference);

                if (stereotypeManagementMode != StereotypeManagementMode.Ignore)
                {
                    SynchronizeStereotypes(
                        incomingElement: incomingElement,
                        packageElement: element,
                        stereotypeManagementMode: stereotypeManagementMode);
                }

                SynchronizeElements(
                    incomingParentId: incomingElement.Id,
                    packageParentId: element.Id,
                    incomingLookup: incomingLookup,
                    packageLookup: packageLookup,
                    idMap: idMap,
                    matchedIds: matchedIds,
                    createAttributesWithUnknownTypes: createAttributesWithUnknownTypes,
                    stereotypeManagementMode: stereotypeManagementMode,
                    addToParent: addToParent);
            }

            IElementPersistable ResolveElement(IElementPersistable incomingElement)
            {
                if (packageLookup.TryGetElementByReference(
                        incomingElement.ExternalReference,
                        incomingElement.SpecializationTypeId,
                        out var packageElement))
                {
                    return packageElement;
                }

                packageElement = candidates
                    .SingleOrDefault(x => !matchedIds.Contains(x.Id) &&
                                          x.Name == incomingElement.Name &&
                                          x.SpecializationTypeId == incomingElement.SpecializationTypeId);
                if (packageElement != null)
                {
                    packageElement.ExternalReference = incomingElement.ExternalReference;
                    return packageElement;
                }

                addToParent(incomingElement);
                return incomingElement;
            }
        }

        private static void SynchronizeStereotypes(IElementPersistable incomingElement, IElementPersistable packageElement, StereotypeManagementMode stereotypeManagementMode)
        {
            foreach (var incomingStereotype in incomingElement.Stereotypes)
            {
                var packageStereotype = packageElement.Stereotypes.FirstOrDefault(x => IsMatch(x, incomingStereotype));
                if (packageStereotype == null)
                {
                    packageElement.Stereotypes.Add(incomingStereotype);
                    continue;
                }

                if (stereotypeManagementMode == StereotypeManagementMode.Fully)
                {
                    packageElement.Stereotypes.Remove(packageStereotype);
                    packageElement.Stereotypes.Add(incomingStereotype);
                }
            }

            static bool IsMatch(IStereotypePersistable packageStereotype, IStereotypePersistable incomingStereotype)
            {
                if (!string.IsNullOrWhiteSpace(incomingStereotype.DefinitionId) &&
                    incomingStereotype.DefinitionId == packageStereotype.DefinitionId)
                {
                    return true;
                }

                if (!string.IsNullOrWhiteSpace(incomingStereotype.DefinitionPackageId) &&
                    incomingStereotype.DefinitionPackageId != packageStereotype.DefinitionPackageId)
                {
                    return false;
                }

                if (!string.IsNullOrWhiteSpace(incomingStereotype.DefinitionPackageName) &&
                    incomingStereotype.DefinitionPackageName != packageStereotype.DefinitionPackageName)
                {
                    return false;
                }

                return incomingStereotype.Name == packageStereotype.Name;
            }
        }

        private static void RemapTypeReferences(
            IEnumerable<IElementPersistable> incomingElements,
            IEnumerable<IAssociationPersistable> incomingAssociations,
            IReadOnlyDictionary<string, string> idMap)
        {
            foreach (var element in incomingElements)
            {
                if (idMap.ContainsKey(element.Id))
                {
                    element.Id = idMap[element.Id];
                }

                if (element.ParentFolderId != null && idMap.ContainsKey(element.ParentFolderId))
                {
                    element.ParentFolderId = idMap[element.ParentFolderId];
                }

                ReMapTypeReference(element.TypeReference);
            }

            foreach (var association in incomingAssociations)
            {
                ReMapTypeReference(association.SourceEnd.TypeReference);
                ReMapTypeReference(association.TargetEnd.TypeReference);
            }

            void ReMapTypeReference(ITypeReferencePersistable typeReference)
            {
                if (typeReference == null)
                {
                    return;
                }

                if (typeReference.TypeId != null &&
                    idMap.ContainsKey(typeReference.TypeId))
                {
                    typeReference.TypeId = idMap[typeReference.TypeId];
                }

                foreach (var genericTypeParameter in typeReference.GenericTypeParameters ?? Enumerable.Empty<ITypeReferencePersistable>())
                {
                    ReMapTypeReference(genericTypeParameter);
                }
            }
        }

        private static void SynchronizeTypeReference(
            ITypeReferencePersistable incoming,
            ITypeReferencePersistable package)
        {
            if (incoming == null)
            {
                return;
            }

            incoming.GenericTypeParameters ??= new List<ITypeReferencePersistable>(0);

            package.TypeId ??= incoming.TypeId;
            package.IsCollection = incoming.IsCollection;
            package.IsNullable = incoming.IsNullable;
            package.IsNavigable = incoming.IsNavigable;
            package.GenericTypeParameters ??= new List<ITypeReferencePersistable>(incoming.GenericTypeParameters.Count);

            while (package.GenericTypeParameters.Count < incoming.GenericTypeParameters.Count)
            {
                package.GenericTypeParameters.Add(null);
            }

            while (package.GenericTypeParameters.Count > incoming.GenericTypeParameters.Count)
            {
                package.GenericTypeParameters.RemoveAt(package.GenericTypeParameters.Count - 1);
            }

            for (var index = 0; index < incoming.GenericTypeParameters.Count; index++)
            {
                var incomingGenericTypeParameter = incoming.GenericTypeParameters[index];
                package.GenericTypeParameters[index] ??= incomingGenericTypeParameter;
                SynchronizeTypeReference(incomingGenericTypeParameter, package.GenericTypeParameters[index]);
            }
        }
    }
}
