using Intent.Metadata.Models;
using Intent.MetadataSynchronizer;
using Intent.Persistence;
using JetBrains.Annotations;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;
using IElementPersistable = Intent.Persistence.IElementPersistable;

namespace Intent.Modules.CSharp.Importer.Importer;


internal static class CSharpImporterExtensions
{
    private const string TypeDefinitionProfileId = "type-definition";
    private const string ServicesProfileId = "services-services";

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
            ProcessClassElements(csConfig, classElementsMap, builderMetadataManager);
        }
        if (csConfig.ImportProfile.MapInterfacesTo != null)
        {
            var interfaceElementsMap = CreateElements(csConfig.ImportProfile.MapInterfacesTo, GetQualifyingInterfaces(csConfig.ImportProfile, csharpTypes.Interfaces), csConfig.TargetFolderId, builderMetadataManager, csConfig.ImportProfile);
            ProcessInterfaceElements(csConfig, interfaceElementsMap, builderMetadataManager);
        }
    }

    private static IList<InterfaceData> GetQualifyingInterfaces(ImportProfileConfig profile, IList<InterfaceData> interfaces)
    {
        // build a lookup of all interface names that are used as base interfaces
        var baseInterfaceNames = interfaces
            .SelectMany(x => x.Interfaces)
            .ToHashSet(StringComparer.Ordinal);

        // build a lookup of interface name to interface data for quick access
        var interfaceLookup = interfaces
            .SelectMany(i => new[]
            {
                (Key: i.Name, Interface: i),
                (Key: $"{i.Namespace}.{i.Name}", Interface: i)
            })
            .ToLookup(x => x.Key, x => x.Interface, StringComparer.Ordinal);

        var qualifyingInterfaces = interfaces;

        if((profile.SkipBaseElementCreation & ImportTypes.Interface) == ImportTypes.Interface)
        {
            // filter out interfaces that are used as base interfaces
            qualifyingInterfaces = [.. interfaces
                .Where(i => !baseInterfaceNames.Contains(i.Name) &&
                            !baseInterfaceNames.Contains($"{i.Namespace}.{i.Name}"))];
        }

        var methodCache = new Dictionary<string, List<MethodData>>(StringComparer.Ordinal);

        if ((profile.MapBaseMethodsToChildTypes & ImportTypes.Interface) == ImportTypes.Interface)
        {
            // recursively gather inherited methods for each qualifying interface
            foreach (var item in qualifyingInterfaces)
            {
                var inheritedMethods = new List<MethodData>();

                foreach (var baseInterfaceName in item.Interfaces)
                {
                    foreach (var baseInterface in interfaceLookup[baseInterfaceName])
                    {
                        inheritedMethods.AddRange(GetAllMethodsRecursive(baseInterface, interfaceLookup, methodCache));
                    }
                }

                if (inheritedMethods.Count > 0)
                {
                    item.Methods = [.. item.Methods, .. inheritedMethods];
                }
            }
        }

        return qualifyingInterfaces;
    }

    private static List<MethodData> GetAllMethodsRecursive(
        InterfaceData interfaceData, 
        ILookup<string, InterfaceData> interfaceLookup, 
        Dictionary<string, List<MethodData>> cache)
    {
        var cacheKey = $"{interfaceData.Namespace}.{interfaceData.Name}";
        
        if (cache.TryGetValue(cacheKey, out var cachedMethods))
        {
            return cachedMethods;
        }
        
        var allMethods = new List<MethodData>(interfaceData.Methods);
        
        foreach (var baseInterfaceName in interfaceData.Interfaces)
        {
            foreach (var baseInterface in interfaceLookup[baseInterfaceName])
            {
                allMethods.AddRange(GetAllMethodsRecursive(baseInterface, interfaceLookup, cache));
            }
        }
        
        cache[cacheKey] = allMethods;
        return allMethods;
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
        CSharpConfig config,
        List<(ClassData ClassData, IElementPersistable Element)> classDataAndBuilders,
        BuilderMetadataManager builderMetadataManager)
    {
        var profile = config.ImportProfile;

        ProcessElementDependencies(profile, classDataAndBuilders.Select(x => ((TypeDeclarationData)x.ClassData, x.Element)).ToList(), builderMetadataManager);

        foreach (var (interfaceData, element) in classDataAndBuilders)
        {
            if (element.SpecializationTypeId == profile.DependencyProfile?.MapClassesTo?.SpecializationTypeId)
            {
                ProcessClassElement(profile.DependencyProfile, builderMetadataManager, interfaceData, element, config);
            }
            else
            {
                ProcessClassElement(profile, builderMetadataManager, interfaceData, element, config);
            }
        }
    }

    private static void ProcessInterfaceElements(
        CSharpConfig config,
        List<(InterfaceData InterfaceData, IElementPersistable Element)> classDataAndBuilders,
        BuilderMetadataManager builderMetadataManager)
    {
        var profile = config.ImportProfile;
        ProcessElementDependencies(profile, classDataAndBuilders.Select(x => ((TypeDeclarationData)x.InterfaceData, x.Element)).ToList(), builderMetadataManager);

        foreach (var (interfaceData, element) in classDataAndBuilders)
        {
            if (element.SpecializationTypeId == profile.DependencyProfile?.MapClassesTo?.SpecializationTypeId)
            {
                ProcessTypeElement(profile.DependencyProfile, builderMetadataManager, interfaceData, element, config);
            }
            else
            {
                ProcessTypeElement(profile, builderMetadataManager, interfaceData, element, config);
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

    private static void ProcessClassElement(ImportProfileConfig profile, BuilderMetadataManager builderMetadataManager, ClassData classData, IElementPersistable element, CSharpConfig config)
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

        ProcessTypeElement(profile, builderMetadataManager, (TypeDeclarationData)classData, element, config);
    }

    private static void ProcessTypeElement(ImportProfileConfig profile, BuilderMetadataManager builderMetadataManager, TypeDeclarationData classData, IElementPersistable element, CSharpConfig config)
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
            var existingMethod = element.ChildElements.SingleOrDefault(x => x.ExternalReference == $"{classData.GetIdentifier()}+{method.GetIdentifier()}");
            var newMethod = existingMethod ?? element.ChildElements.Add(
                id: Guid.NewGuid().ToString().ToLower(),
                specializationType: profile.MapMethodsTo.SpecializationType,
                specializationTypeId: profile.MapMethodsTo.SpecializationTypeId,
                name: method.Name,
                parentId: element.Id,
                externalReference: $"{classData.GetIdentifier()}+{method.GetIdentifier()}");

            builderMetadataManager.SetTypeReference(newMethod, method.ReturnType, method.ReturnType?.Contains('?') ?? false, method.ReturnsCollection);
            ApplySyncAsyncStereotypes(profile, newMethod, method, config);

            foreach (var parameter in method.Parameters)
            {
                if (profile.MapMethodParametersTo is null || parameter.IsCancellationToken())
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

    private static bool IsServicesProfile(ImportProfileConfig profile)
    {
        return profile.Identifier == ServicesProfileId;
    }

    private static void ApplyCSharpStereotype(IElementPersistable element, string csharpNamespace)
    {

        const string CSharpStereotypeDefinitionId = "30c5995e-17ab-4cc7-8881-3e9561ab06fe";
        const string NamespacePropertyDefinitionId = "8CCCD630-6AF9-41FA-8E5F-414860AD89BB";

        var stereotype = element.GetOrCreateStereotype(CSharpStereotypeDefinitionId, "C#", "730e1275-0c32-44f7-991a-9619d07ca68d", "Intent.Common.CSharp");
        var property = stereotype.GetOrCreateProperty(NamespacePropertyDefinitionId, "Namespace", csharpNamespace);
        
        property.Value = csharpNamespace;
    }

    private static void ApplySyncAsyncStereotypes(ImportProfileConfig profile, IElementPersistable element, MethodData method, CSharpConfig config)
    {
        // if not service import, or the preserve async option is not enabled, then we don't want to apply any stereotypes and can return early
        // This will default to async methods with cancellation tokens
        if (!IsServicesProfile(profile) || !config.PreserveAsync)
        {
            return;
        }

        // if the method is not async, then we can apply the synchronous stereotype and return early
        if (!method.IsAsync)
        {
            const string synchronousStereotypeId = "2db1104b-ca3c-47a6-ad82-a0d2ee915c06";
            element.GetOrCreateStereotype(synchronousStereotypeId, "Synchronous", "b258d75f-f895-43b9-bb91-6500664716f9", "Intent.Application.Contracts");

            return;
        }

        if(!method.Parameters.Any(x => x.IsCancellationToken()))
        {
            const string asynchronousStereotypeId = "A225C795-33E9-417D-8D58-E22826A08224";
            const string suppressCancellationTokenId = "2801e2a9-5797-406f-b289-43af8fbb2d7e";

            var stereotype = element.GetOrCreateStereotype(asynchronousStereotypeId, "Asynchronous", "b258d75f-f895-43b9-bb91-6500664716f9", "Intent.Application.Contracts");
            var property = stereotype.GetOrCreateProperty(suppressCancellationTokenId, "Suppress Cancellation Token", "true");

            return;
        }

    }    

    private static bool IsCancellationToken(this ParameterData parameter)
    {
        HashSet<string> ignoredParameterTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "System.Threading.CancellationToken",
            "Cancellationtoken"
        };

        return ignoredParameterTypes.Contains(parameter?.Type ?? "");
    }
}