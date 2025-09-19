using Intent.MetadataSynchronizer.CSharp.CLI.Builders;
using Intent.Modules.Common.Templates;
using Intent.Persistence;
using IElementPersistable = Intent.Persistence.IElementPersistable;

namespace Intent.MetadataSynchronizer.CSharp.CLI;


internal static class PersistableFactory
{
    public static Persistables GetPersistables(CSharpConfig csConfig, CoreTypesData coreTypeElements,
        IPackageModelPersistable package)
    {
        if (!string.IsNullOrWhiteSpace(csConfig.TargetFolderId) && package.GetElementById(csConfig.TargetFolderId) is null)
        {
            throw new InvalidOperationException($"{nameof(csConfig.TargetFolderId)} has value of '{csConfig.TargetFolderId}' which could not be found.");
        }

        //var elements = new List<IElementPersistable>();

        var classDataLookup = coreTypeElements.Classes.ToDictionary(classData => classData.GetIdentifier());

        var builderMetadataManager = new BuilderMetadataManager(package, csConfig);

        var classDataAndBuilders = RegisterElements(coreTypeElements, csConfig.TargetFolderId, builderMetadataManager);
        PostProcessElements(csConfig.ImportProfile, classDataAndBuilders, classDataLookup, builderMetadataManager);

        return builderMetadataManager.GetPersistables();
    }

    private static List<(ClassData, IElementPersistable)> RegisterElements(
        CoreTypesData coreTypeElements,
        string? targetFolderId,
        BuilderMetadataManager builderMetadataManager)
    {
        var result = new List<(ClassData, IElementPersistable)>();

        foreach (var classData in coreTypeElements.Classes)
        {
            var element = builderMetadataManager.GetElementByReference(classData.GetIdentifier()) ?? builderMetadataManager.CreateElement(classData, targetFolderId);
            result.Add((classData, element));
        }

        return result;
    }
    private static void PostProcessElements(
        ImportProfileConfig profile,
        List<(ClassData, IElementPersistable)> classDataAndBuilders,
        Dictionary<string, ClassData> classDataLookup,
        BuilderMetadataManager builderMetadataManager)
    {
        // handle associations first, so we can get a list of foreign keys to be used just below
        List<IAssociationPersistable> associations = [];
        foreach (var classData in classDataLookup.Values)
        {
            var propertiesThatAreAssociations = classData.Properties.Where(p => builderMetadataManager.GetElementByReference(p.Type)?.SpecializationTypeId == profile.MapClassesTo.SpecializationTypeId).ToArray();
            foreach (var property in propertiesThatAreAssociations)
            {
                if (!classDataLookup.TryGetValue(property.Type!, out var otherClass))
                {
                    continue;
                }
                var existingAssociation = builderMetadataManager.GetAssociationByReference($"{classData.GetIdentifier()}+{property.GetIdentifier()}");
                
                // Is this a bi-directional association?
                var bidirectionalProperty = otherClass.Properties
                    .FirstOrDefault(p => builderMetadataManager.GetElementByReference(p.Type)?.SpecializationTypeId == profile.MapClassesTo.SpecializationTypeId &&
                                         classDataLookup.TryGetValue(property.Type!, out var thisClass) &&
                                         thisClass == otherClass && p.Type == classData.GetIdentifier());

                var sourceElement = builderMetadataManager.GetElementByReference(classData.GetIdentifier());
                var targetElement = builderMetadataManager.GetElementByReference(property.Type);
                var association = existingAssociation ?? builderMetadataManager.CreateAssociation(sourceElement.Id, targetElement.Id);

                association.ExternalReference = $"{classData.GetIdentifier()}+{property.GetIdentifier()}";
                if (bidirectionalProperty is null)
                {
                    // if the class contains a FK to the other class PK (based on convention)
                    var associationKeys = classData.Properties.Where(p => p.Name == $"{otherClass.Name}Id");
                    association.SourceEnd.TypeReference.IsCollection = associationKeys.Any(); // this is a bit weird.
                }
                else
                {
                    association.SourceEnd.Name = bidirectionalProperty.Name;
                    association.SourceEnd.TypeReference.IsNullable = bidirectionalProperty.IsNullable;
                    association.SourceEnd.TypeReference.IsCollection = bidirectionalProperty.IsCollection;
                }

                association.TargetEnd.TypeReference.TypeId = targetElement.Id;
                association.TargetEnd.Name = property.Name;
                association.TargetEnd.TypeReference.IsNullable = property.IsNullable;
                association.TargetEnd.TypeReference.IsCollection = property.IsCollection;
            }
        }

        foreach (var (classData, element) in classDataAndBuilders)
        {
            foreach (var constructor in classData.Constructors)
            {
                if (profile.MapConstructorsTo is null)
                {
                    break;
                }

                var existingCtor = element.ChildElements.SingleOrDefault(x => x.ExternalReference == $"{classData.GetIdentifier()}+{constructor.GetIdentifier()}");
                var newCtor = existingCtor ?? element.ChildElements.Add(
                    id: Guid.NewGuid().ToString().ToLower(),
                    specializationType: profile.MapConstructorsTo.SpecializationType,
                    specializationTypeId: profile.MapConstructorsTo.SpecializationTypeId,
                    name: element.Name,
                    parentId: element.Id,
                    externalReference: $"{classData.GetIdentifier()}+{constructor.GetIdentifier()}");
                foreach (var parameter in constructor.Parameters)
                {
                    if (profile.MapConstructorParametersTo is null)
                    {
                        break;
                    }
                    var existingParam = newCtor.ChildElements.SingleOrDefault(x => x.ExternalReference == $"{classData.GetIdentifier()}+{parameter.GetIdentifier()}");
                    var newParam = existingParam ?? newCtor.ChildElements.Add(
                        id: Guid.NewGuid().ToString().ToLower(),
                        specializationType: profile.MapConstructorParametersTo.SpecializationType,
                        specializationTypeId: profile.MapConstructorParametersTo.SpecializationTypeId,
                        name: parameter.Name,
                        parentId: newCtor.Id,
                        externalReference: $"{classData.GetIdentifier()}+{parameter.GetIdentifier()}");

                    // a shit pattern from the original importer:
                    builderMetadataManager.SetTypeReference(newParam, parameter.Type, parameter.IsNullable, parameter.IsCollection);
                }
            }

            foreach (var prop in classData.Properties.Where(p => builderMetadataManager.GetElementByReference(p.Type)?.SpecializationTypeId != profile.MapClassesTo.SpecializationTypeId))
            {
                if (profile.MapPropertiesTo is null)
                {
                    break;
                }
                var existingAttribute = element.ChildElements.SingleOrDefault(x => x.ExternalReference == $"{classData.GetIdentifier()}+{prop.GetIdentifier()}");
                var attribute = existingAttribute ?? element.ChildElements.Add(
                    id: Guid.NewGuid().ToString().ToLower(),
                    specializationType: profile.MapPropertiesTo.SpecializationType,
                    specializationTypeId: profile.MapPropertiesTo.SpecializationTypeId,
                    name: prop.Name,
                    parentId: element.Id,
                    externalReference: $"{classData.GetIdentifier()}+{prop.GetIdentifier()}");
                builderMetadataManager.SetTypeReference(attribute, prop.Type, prop.IsNullable, prop.IsCollection);

                // if the property is flagged with the [Key] atribute, is called "Id", or is called {ClassName}Id - then assume its a PK
                //if (prop.Attributes.Contains("Key") || prop.Name.Equals("Id", StringComparison.InvariantCultureIgnoreCase) ||
                //    prop.Name.Equals($"{classData.Name}Id", StringComparison.InvariantCultureIgnoreCase))
                //{
                //    attBuilder.AddStereotype("b99aac21-9ca4-467f-a3a6-046255a9eed6", "Primary Key", []);
                //}

                // check if there are any associations where the source is this class, and target end is a table which matches the current column
                //var qualifyingAssociations = associations.Where(a =>
                //{
                //    if (!builderMetadataManager.TryGetElementById(a.TargetEnd.TypeReference.TypeId, out var targetElement))
                //    {
                //        return false;
                //    }

                //    // this use case caters for when the source end has the navigation property and the foreign key.
                //    if (a.SourceEnd.TypeReference?.TypeName == classData.Name &&
                //        targetElement.Name == prop.Name.Replace("Id", ""))
                //    {
                //        return true;
                //    }

                //    // this caters for when the target end has the foreign key and the navigation property is in the source end 
                //    if (!builderMetadataManager.TryGetElementById(a.SourceEnd?.TypeReference?.TypeId, out var sourceElement))
                //    {
                //        return false;
                //    }

                //    return targetElement.Name == classData.Name && sourceElement.Name == prop.Name.Replace("Id", "");
                //});

                //if (qualifyingAssociations.Any())
                //{
                //    foreach (var association in qualifyingAssociations)
                //    {
                //        attBuilder.AddStereotype("793a5128-57a1-440b-a206-af5722b752a6", "Foreign Key",
                //        [
                //            new StereotypeProperty("42e4f9b5-f834-4e5f-86aa-d3a35c505076", Display: "Association", Value: association.TargetEnd.Id)
                //        ]);
                //    }
                //}
            }

            foreach (var method in classData.Methods)
            {
                if (profile.MapMethodsTo is null)
                {
                    break;
                }
                var existingMethod = element.ChildElements.SingleOrDefault(x => x.ExternalReference == method.GetIdentifier());
                var newMethod = existingMethod ?? element.ChildElements.Add(
                    id: Guid.NewGuid().ToString().ToLower(),
                    specializationType: profile.MapMethodsTo.SpecializationType,
                    specializationTypeId: profile.MapMethodsTo.SpecializationTypeId,
                    name: element.Name,
                    parentId: element.Id,
                    externalReference: $"{classData.GetIdentifier()}+{method.GetIdentifier()}");
                foreach (var parameter in method.Parameters)
                {
                    if (profile.MapMethodParametersTo is null)
                    {
                        break;
                    }
                    var existingParam = newMethod.ChildElements.SingleOrDefault(x => x.ExternalReference == parameter.GetIdentifier());
                    var newParam = existingParam ?? newMethod.ChildElements.Add(
                        id: Guid.NewGuid().ToString().ToLower(),
                        specializationType: profile.MapMethodParametersTo.SpecializationType,
                        specializationTypeId: profile.MapMethodParametersTo.SpecializationTypeId,
                        name: parameter.Name,
                        parentId: newMethod.Id,
                        externalReference: parameter.Name);

                    // a shit pattern from the original importer:
                    builderMetadataManager.SetTypeReference(newParam, parameter.Type, parameter.IsNullable, parameter.IsCollection);
                }
            }
        }
    }
}