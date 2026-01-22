using Intent.MetadataSynchronizer;
using Intent.Persistence;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using static System.Runtime.InteropServices.JavaScript.JSType;
using IElementPersistable = Intent.Persistence.IElementPersistable;

namespace Intent.Modules.CSharp.Importer.Importer;


internal static class CSharpImporterExtensions
{
    private const string TypeDefinitionProfileId = "type-definition";

    public static void ImportCSharpTypes(this IPackageModelPersistable package, CoreTypesData csharpTypes, CSharpConfig csConfig)
    {
        if (!string.IsNullOrWhiteSpace(csConfig.TargetFolderId) && package.GetElementById(csConfig.TargetFolderId) is null)
        {
            throw new InvalidOperationException($"{nameof(csConfig.TargetFolderId)} has value of '{csConfig.TargetFolderId}' which could not be found.");
        }

        var builderMetadataManager = new BuilderMetadataManager(package, csConfig);

        if (csConfig.ImportProfile.MapEnumsTo != null)
        {
            CreateEnums(csConfig.ImportProfile, csharpTypes.Enums, csConfig.TargetFolderId, builderMetadataManager);
        }
        if (csConfig.ImportProfile.MapClassesTo != null)
        {
            var classElementsMap = CreateElements(csConfig.ImportProfile.MapClassesTo, csharpTypes.Classes, csConfig.TargetFolderId, builderMetadataManager, csConfig.ImportProfile);
            ProcessClassElements(csConfig.ImportProfile, classElementsMap, builderMetadataManager);
        }
        if (csConfig.ImportProfile.MapInterfacesTo != null)
        {
            var interfaceElementsMap = CreateElements(csConfig.ImportProfile.MapInterfacesTo, csharpTypes.Interfaces, csConfig.TargetFolderId, builderMetadataManager, csConfig.ImportProfile);
            ProcessInterfaceElements(csConfig.ImportProfile, interfaceElementsMap, builderMetadataManager);
        }
    }

    private static List<(ClassData, IElementPersistable)> CreateElements(
        IElementSettings settings,
        IList<ClassData> csharpTypes,
        string? targetFolderId,
        BuilderMetadataManager builderMetadataManager,
        ImportProfileConfig profile)
    {
        var result = new List<(ClassData, IElementPersistable)>();

        foreach (var classData in csharpTypes)
        {
            var element = builderMetadataManager.GetElementByReference(classData.GetIdentifier())
                          ?? builderMetadataManager.GetElementByName(classData.Name) 
                          ?? builderMetadataManager.CreateElement(settings, classData.Name, classData.FilePath, classData.GetIdentifier(), targetFolderId);
            
            // Apply C# stereotype with namespace only for all-types-as-type-definition profile
            if (IsTypeDefinitionProfile(profile))
            {
                ApplyCSharpStereotype(element, classData.Namespace);
            }
            
            result.Add((classData, element));
        }

        return result;
    }

    private static List<(InterfaceData, IElementPersistable)> CreateElements(
        IElementSettings settings,
        IList<InterfaceData> csharpTypes,
        string? targetFolderId,
        BuilderMetadataManager builderMetadataManager,
        ImportProfileConfig profile)
    {
        var result = new List<(InterfaceData, IElementPersistable)>();

        foreach (var classData in csharpTypes)
        {
            var element = builderMetadataManager.GetElementByReference(classData.GetIdentifier())
                          ?? builderMetadataManager.GetElementByName(classData.Name)
                          ?? builderMetadataManager.CreateElement(settings, classData.Name, classData.FilePath, classData.GetIdentifier(), targetFolderId);
            
            // Apply C# stereotype with namespace only for all-types-as-type-definition profile
            if (IsTypeDefinitionProfile(profile))
            {
                ApplyCSharpStereotype(element, classData.Namespace);
            }
            
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
                          ?? builderMetadataManager.GetElementByName(enumData.Name)
                          ?? builderMetadataManager.CreateElement(profile.MapEnumsTo, enumData.Name, enumData.FilePath, enumData.GetIdentifier(), targetFolderId);
            
            // Apply C# stereotype with namespace only for all-types-as-type-definition profile
            if (IsTypeDefinitionProfile(profile))
            {
                ApplyCSharpStereotype(element, enumData.Namespace);
            }
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

    private static void ProcessClassElements(
        ImportProfileConfig profile,
        List<(ClassData ClassData, IElementPersistable Element)> classDataAndBuilders,
        BuilderMetadataManager builderMetadataManager)
    {
        ProcessElementDependencies(profile, classDataAndBuilders.Select(x => ((TypeDeclarationData)x.ClassData, x.Element)).ToList(), builderMetadataManager);

        foreach (var (interfaceData, element) in classDataAndBuilders)
        {
            if (element.SpecializationTypeId == profile.DependencyProfile?.MapClassesTo?.SpecializationTypeId)
            {
                ProcessClassElement(profile.DependencyProfile, builderMetadataManager, interfaceData, element);
            }
            else
            {
                ProcessClassElement(profile, builderMetadataManager, interfaceData, element);
            }
        }
    }

    private static void ProcessInterfaceElements(
        ImportProfileConfig profile,
        List<(InterfaceData InterfaceData, IElementPersistable Element)> classDataAndBuilders,
        BuilderMetadataManager builderMetadataManager)
    {
        ProcessElementDependencies(profile, classDataAndBuilders.Select(x => ((TypeDeclarationData)x.InterfaceData, x.Element)).ToList(), builderMetadataManager);

        foreach (var (interfaceData, element) in classDataAndBuilders)
        {
            if (element.SpecializationTypeId == profile.DependencyProfile?.MapClassesTo?.SpecializationTypeId)
            {
                ProcessTypeElement(profile.DependencyProfile, builderMetadataManager, interfaceData, element);
            }
            else
            {
                ProcessTypeElement(profile, builderMetadataManager, interfaceData, element);
            }
        }
    }

    private static void ProcessElementDependencies(ImportProfileConfig profile, List<(TypeDeclarationData TypeData, IElementPersistable Element)> classDataAndBuilders, BuilderMetadataManager builderMetadataManager)
    {
        var classDataLookup = classDataAndBuilders.Select(x => x.TypeData).ToDictionary(classData => classData.GetIdentifier());

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
    }

    private static void ProcessClassElement(ImportProfileConfig profile, BuilderMetadataManager builderMetadataManager, ClassData classData, IElementPersistable element)
    {
        var baseType = classData.BaseType != null ? builderMetadataManager.GetElementByReference(classData.BaseType) : null;
        if (baseType != null && profile.MapInheritanceTo != null)
        {
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

        ProcessTypeElement(profile, builderMetadataManager, (TypeDeclarationData)classData, element);
    }

    private static void ProcessTypeElement(ImportProfileConfig profile, BuilderMetadataManager builderMetadataManager, TypeDeclarationData classData, IElementPersistable element)
    {
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
                name: method.Name,
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

    private static bool IsTypeDefinitionProfile(ImportProfileConfig profile)
    {
        return profile.Identifier == TypeDefinitionProfileId;
    }

    private static void ApplyCSharpStereotype(IElementPersistable element, string csharpNamespace)
    {
        if (element is not ElementPersistable persistable)
        {
            return;
        }

        const string CSharpStereotypeDefinitionId = "30c5995e-17ab-4cc7-8881-3e9561ab06fe";
        const string NamespacePropertyDefinitionId = "8CCCD630-6AF9-41FA-8E5F-414860AD89BB";
        
        var stereotype = persistable.GetOrCreateStereotype(
            CSharpStereotypeDefinitionId,
            init =>
            {
                init.Name = "C#";
                init.DefinitionPackageId = "730e1275-0c32-44f7-991a-9619d07ca68d";
                init.DefinitionPackageName = "Intent.Common.CSharp";
            });
        
        stereotype.GetOrCreateProperty(NamespacePropertyDefinitionId, prop =>
        {
            prop.Name = "Namespace";
        }).Value = csharpNamespace;
    }
}