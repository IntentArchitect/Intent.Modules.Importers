using Intent.MetadataSynchronizer;
using Intent.Persistence;
using IElementPersistable = Intent.Persistence.IElementPersistable;

namespace Intent.Modules.CSharp.Importer.Importer;


internal static class CSharpImporterExtensions
{
    public static void ImportCSharpTypes(this IPackageModelPersistable package, CoreTypesData csharpTypes, CSharpConfig csConfig)
    {
        if (!string.IsNullOrWhiteSpace(csConfig.TargetFolderId) && package.GetElementById(csConfig.TargetFolderId) is null)
        {
            throw new InvalidOperationException($"{nameof(csConfig.TargetFolderId)} has value of '{csConfig.TargetFolderId}' which could not be found.");
        }

        var classDataLookup = csharpTypes.Classes.ToDictionary(classData => classData.GetIdentifier());

        var builderMetadataManager = new BuilderMetadataManager(package, csConfig);

        if (csConfig.ImportProfile.MapEnumsTo != null)
        {
            CreateEnums(csConfig.ImportProfile, csharpTypes.Enums, csConfig.TargetFolderId, builderMetadataManager);
        }
        if (csConfig.ImportProfile.MapClassesTo != null)
        {
            var classDataAndBuilders = CreateElements(csharpTypes.Classes, csConfig.TargetFolderId, builderMetadataManager);
            PostProcessElements(csConfig.ImportProfile, classDataAndBuilders, classDataLookup, builderMetadataManager);
        }
    }

    private static List<(ClassData, IElementPersistable)> CreateElements(
        IList<ClassData> csharpTypes,
        string? targetFolderId,
        BuilderMetadataManager builderMetadataManager)
    {
        var result = new List<(ClassData, IElementPersistable)>();

        foreach (var classData in csharpTypes)
        {
            var element = builderMetadataManager.GetElementByReference(classData.GetIdentifier()) 
                          ?? builderMetadataManager.CreateElement(classData, targetFolderId);
            result.Add((classData, element));
        }

        return result;
    }

    private static void CreateEnums(ImportProfileConfig profile, IList<EnumData> enums,
        string? targetFolderId,
        BuilderMetadataManager builderMetadataManager)
    {
        foreach (var enumData in enums)
        {
            var element = builderMetadataManager.GetElementByReference(enumData.GetIdentifier())
                          ?? builderMetadataManager.CreateElement(profile.MapEnumsTo, enumData.Name, enumData.FilePath, enumData.GetIdentifier(), targetFolderId);
            var settings = profile.MapEnumLiteralsTo;
            if (settings != null)
            {
                foreach (var literal in enumData.Literals)
                {
                    var existingLiteral = element.ChildElements.SingleOrDefault(x => x.ExternalReference == $"{enumData.GetIdentifier()}+{literal.GetIdentifier()}")
                                          ?? element.ChildElements.SingleOrDefault(x => x.Name == literal.Name);
                    var newLiteral = existingLiteral ?? element.ChildElements.Add(
                        id: Guid.NewGuid().ToString().ToLower(),
                        specializationType: settings.SpecializationType,
                        specializationTypeId: settings.SpecializationTypeId,
                        name: literal.Name,
                        parentId: element.Id,
                        externalReference: $"{enumData.GetIdentifier()}+{literal.GetIdentifier()}");

                    newLiteral.Value = literal.Value;
                }
            }
        }
    }

    private static void PostProcessElements(
        ImportProfileConfig profile,
        List<(ClassData, IElementPersistable)> classDataAndBuilders,
        Dictionary<string, ClassData> classDataLookup,
        BuilderMetadataManager builderMetadataManager)
    {
        foreach (var classData in classDataLookup.Values)
        {
            var propertiesThatAreAssociations = classData.Properties.Where(p => 
                builderMetadataManager.GetElementByReference(p.Type)?.SpecializationTypeId == profile.MapClassesTo!.SpecializationTypeId).ToArray();
            foreach (var property in propertiesThatAreAssociations)
            {
                if (!classDataLookup.TryGetValue(property.Type!, out var otherClass))
                {
                    continue;
                }

                var targetElement = builderMetadataManager.GetElementByReference(property.Type!)!;
                if (profile.DependencyProfile?.MapClassesTo != null)
                {
                    targetElement.SpecializationType = profile.DependencyProfile.MapClassesTo.SpecializationType;
                    targetElement.SpecializationTypeId = profile.DependencyProfile.MapClassesTo.SpecializationTypeId;
                }

                if (profile.MapAssociationsTo == null)
                {
                    continue;
                }

                var existingAssociation = builderMetadataManager.GetAssociationByReference($"{classData.GetIdentifier()}+{property.GetIdentifier()}");

                var sourceElement = builderMetadataManager.GetElementByReference(classData.GetIdentifier())!;
                var association = existingAssociation ?? builderMetadataManager.CreateAssociation(profile.MapAssociationsTo, sourceElement.Id, targetElement.Id);

                association.ExternalReference = $"{classData.GetIdentifier()}+{property.GetIdentifier()}";

                // Is this a bi-directional association?
                var bidirectionalProperty = otherClass.Properties
                    .FirstOrDefault(p => builderMetadataManager.GetElementByReference(p.Type)?.SpecializationTypeId == profile.MapClassesTo.SpecializationTypeId &&
                                         classDataLookup.TryGetValue(property.Type!, out var thisClass) &&
                                         thisClass == otherClass && p.Type == classData.GetIdentifier());
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
            if (element.SpecializationTypeId == profile.DependencyProfile?.MapClassesTo?.SpecializationTypeId)
            {
                ProcessElement(profile.DependencyProfile, builderMetadataManager, classData, element);
            }
            else
            {
                ProcessElement(profile, builderMetadataManager, classData, element);
            }
        }
    }

    private static void ProcessElement(ImportProfileConfig profile, BuilderMetadataManager builderMetadataManager, ClassData classData, IElementPersistable element)
    {
        var baseType = classData.BaseType != null ? builderMetadataManager.GetElementByReference(classData.BaseType) : null;
        if (baseType != null && profile.MapInheritanceTo != null) {
            var existingAssociation = builderMetadataManager.GetAssociationByReference($"{classData.GetIdentifier()}+extends+{classData.BaseType}");

            var sourceElement = builderMetadataManager.GetElementByReference(classData.GetIdentifier())!;
            var association = existingAssociation ?? builderMetadataManager.CreateAssociation(profile.MapInheritanceTo, sourceElement.Id, baseType.Id);

            association.ExternalReference = $"{classData.GetIdentifier()}+extends+{classData.BaseType}";
        }


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

        foreach (var prop in classData.Properties.Where(p =>
                     profile.MapAssociationsTo == null || // was not handled as an association.
                     builderMetadataManager.GetElementByReference(p.Type)?.SpecializationTypeId != profile.MapClassesTo.SpecializationTypeId))
        {
            if (profile.MapPropertiesTo is null)
            {
                break;
            }
            var existingAttribute = element.ChildElements.SingleOrDefault(x => x.ExternalReference == $"{classData.GetIdentifier()}+{prop.GetIdentifier()}")
                                    ?? element.ChildElements.SingleOrDefault(x => x.Name == classData.Name);
            var attribute = existingAttribute ?? element.ChildElements.Add(
                id: Guid.NewGuid().ToString().ToLower(),
                specializationType: profile.MapPropertiesTo.SpecializationType,
                specializationTypeId: profile.MapPropertiesTo.SpecializationTypeId,
                name: prop.Name,
                parentId: element.Id,
                externalReference: $"{classData.GetIdentifier()}+{prop.GetIdentifier()}");
            builderMetadataManager.SetTypeReference(attribute, prop.Type, prop.IsNullable, prop.IsCollection);
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