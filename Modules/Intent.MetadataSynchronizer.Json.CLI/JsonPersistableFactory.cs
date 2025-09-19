using System.Linq;
using System.Text.Json;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.Templates;

namespace Intent.MetadataSynchronizer.Json.CLI;

/// <summary>
/// Factory class for creating persistables from JSON files.
/// This class can be used by both the CLI and modules.
/// </summary>
public static class JsonPersistableFactory
{
    public static Persistables GetPersistables(
        JsonConfig config,
        IReadOnlyCollection<PackageModelPersistable> packages)
    {
        // Default behavior: scan all JSON files in the source folder
        return GetPersistablesCore(config, packages, selectedFiles: null);
    }

    public static Persistables GetPersistables(
        JsonConfig config,
        IReadOnlyCollection<PackageModelPersistable> packages,
        IReadOnlyCollection<string> selectedFiles)
    {
        // Use only the selected files
        return GetPersistablesCore(config, packages, selectedFiles);
    }

    private static Persistables GetPersistablesCore(
        JsonConfig config,
        IReadOnlyCollection<PackageModelPersistable> packages,
        IReadOnlyCollection<string>? selectedFiles)
    {
        // Resolve settings from profile
        var settings = ProfileFactory.GetSettings(config.Profile);
        var lookups = new MetadataLookup(packages);
        if (!lookups.TryGetTypeDefinitionByName("bool", 0, out var boolType)) throw new Exception();
        if (!lookups.TryGetTypeDefinitionByName("object", 0, out var objectType)) throw new Exception();
        if (!lookups.TryGetTypeDefinitionByName("string", 0, out var stringType)) throw new Exception();
        if (!lookups.TryGetTypeDefinitionByName("decimal", 0, out var numberType)) throw new Exception();
        if (!lookups.TryGetTypeDefinitionByName("guid", 0, out var guidType)) throw new Exception();
        if (!lookups.TryGetTypeDefinitionByName("datetime", 0, out var dateTimeType)) throw new Exception();

        var typeLookups = new Dictionary<ClassificationType, ElementPersistable>
        {
            [ClassificationType.Bool] = boolType,
            [ClassificationType.String] = stringType,
            [ClassificationType.Object] = objectType,
            [ClassificationType.Unknown] = objectType,
            [ClassificationType.Number] = numberType,
            [ClassificationType.Guid] = guidType,
            [ClassificationType.DateTime] = dateTimeType,
        };

        var classElements = new List<ClassElement>(20);
        var associations = new List<AssociationPersistable>(20);
        var createdFolders = new Dictionary<string, ElementPersistable>(); // Track created folders by relative path

        // Determine which files to process
        IEnumerable<string> filesToProcess;
        if (selectedFiles != null)
        {
            // Use the provided selected files
            filesToProcess = selectedFiles.Where(File.Exists);
        }
        else
        {
            // Default behavior: scan all JSON files in the source folder
            filesToProcess = Directory.GetFiles(config.SourceJsonFolder, "*.json");
        }

        foreach (var jsonFile in filesToProcess)
        {
            var document = JsonDocument.Parse(File.ReadAllText(jsonFile), new JsonDocumentOptions
            {
                CommentHandling = JsonCommentHandling.Skip
            });

            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(jsonFile);

            // Calculate relative path from source folder and create folder structure
            var relativePath = Path.GetRelativePath(config.SourceJsonFolder, jsonFile);
            var directoryPath = Path.GetDirectoryName(relativePath);

            string? parentFolderId = null;

            // Create nested folder structure if the file is in subdirectories
            if (!string.IsNullOrEmpty(directoryPath) && directoryPath != ".")
            {
                parentFolderId = CreateFolderHierarchy(directoryPath, createdFolders, classElements);
            }

            var elementPersistable = ElementPersistable.Create(
                specializationType: settings.RootSpecializationType,
                specializationTypeId: settings.RootSpecializationTypeId,
                name: Casing(config, fileNameWithoutExtension),
                parentId: parentFolderId,
                externalReference: relativePath.Replace('\\', '/')); // Use relative path as external reference

            classElements.Add(new (elementPersistable, new Stack<string>(new[] { fileNameWithoutExtension })));

            // Parse the document and add to classElements and associations
            var (parsedClassElements, parsedAssociations) = Parse(
                config: config,
                jsonElement: document.RootElement,
                intentElement: elementPersistable,
                path: fileNameWithoutExtension,
                pathParts: [Casing(config, fileNameWithoutExtension)],
                profileSettings: settings,
                typeLookups: typeLookups,
                isRootElement: true,
                parentFolderId: parentFolderId);
            
            classElements.AddRange(parsedClassElements);
            associations.AddRange(parsedAssociations);
        }

        do
        {
            var duplicates = classElements.GroupBy(x => x.Element.Name)
                .Where(x => x.Count() > 1)
                .SelectMany(x => x)
                .ToArray();

            if (!duplicates.Any())
            {
                break;
            }

            foreach (var item in duplicates)
            {
                if (!item.PathParts.TryPop(out var part))
                {
                    throw new Exception();
                }

                item.Element.Name = $"{part}{item.Element.Name.ToPascalCase()}";
            }
        } while (true);

        return new Persistables(classElements.Select(x => x.Element).ToArray(), associations);
    }

    private static string Casing(JsonConfig config, string name)
    {
        return config.CasingConvention == CasingConvention.AsIs ? name : name.ToPascalCase();
    }

    private record ParseResult(List<ClassElement> ClassElements, List<AssociationPersistable> Associations);
    private record ClassElement(ElementPersistable Element, Stack<string> PathParts);
    
    private static ParseResult Parse(
        JsonConfig config,
        JsonElement jsonElement,
        ElementPersistable intentElement,
        string path,
        IReadOnlyCollection<string> pathParts,
        ProfileSettings profileSettings,
        Dictionary<ClassificationType, ElementPersistable> typeLookups,
        bool isRootElement = false,
        string? parentFolderId = null)
    {
        var classElements = new List<ClassElement>();
        var associations = new List<AssociationPersistable>();
        
        foreach (var property in jsonElement.EnumerateObject())
        {
            var classification = Classify(property.Value, $"{path}.{property.Name}");
            if (classification.Type == ClassificationType.Object)
            {
                var targetElement = ElementPersistable.Create(
                    specializationType: profileSettings.ComplexTypeSpecializationType,
                    specializationTypeId: profileSettings.ComplexTypeSpecializationTypeId,
                    name: Casing(config, property.Name).Singularize(false),
                    // Place composites/nested elements in the same folder as the root element
                    parentId: parentFolderId,
                    externalReference: classification.Path);

                classElements.Add(new (targetElement, new Stack<string>(pathParts)));

                if (profileSettings.CreateAssociations)
                {
                    var association = new AssociationPersistable
                    {
                        Id = Guid.NewGuid().ToString().ToLower(),
                        SourceEnd = new AssociationEndPersistable
                        {
                            SpecializationType = profileSettings.AssociationSourceEndSpecializationType,
                            SpecializationTypeId = profileSettings.AssociationSourceEndSpecializationTypeId,
                            Name = intentElement.Name,
                            TypeReference = TypeReferencePersistable.Create(
                                typeId: intentElement.Id,
                                isNavigable: false,
                                isNullable: false,
                                isCollection: false,
                                isRequired: default,
                                comment: default,
                                genericTypeId: default,
                                typePackageName: default,
                                typePackageId: default,
                                stereotypes: new List<StereotypePersistable>(),
                                genericTypeParameters: new List<TypeReferencePersistable>()),
                            ExternalReference = path,
                        },
                        TargetEnd = new AssociationEndPersistable
                        {
                            SpecializationType = profileSettings.AssociationTargetEndSpecializationType,
                            SpecializationTypeId = profileSettings.AssociationTargetEndSpecializationTypeId,
                            Name = Casing(config, property.Name),
                            TypeReference = TypeReferencePersistable.Create(
                                typeId: targetElement.Id,
                                isNavigable: true,
                                isNullable: false,
                                isCollection: classification.IsCollection,
                                isRequired: default,
                                comment: default,
                                genericTypeId: default,
                                typePackageName: default,
                                typePackageId: default,
                                stereotypes: new List<StereotypePersistable>(),
                                genericTypeParameters: new List<TypeReferencePersistable>()),
                            ExternalReference = classification.Path,
                        },
                        AssociationType = profileSettings.AssociationSpecializationType,
                        AssociationTypeId = profileSettings.AssociationSpecializationTypeId
                    };
                    associations.Add(association);
                }
                else
                {
                    // If the parent is a ComplexType (e.g., Eventing DTO), use the ComplexTypeAttribute specialization
                    var isParentComplex = intentElement.SpecializationTypeId == profileSettings.ComplexTypeSpecializationTypeId;
                    var complexPropSpec = isParentComplex
                        ? profileSettings.ComplexTypeAttributeSpecializationType
                        : profileSettings.AttributeSpecializationType;
                    var complexPropSpecId = isParentComplex
                        ? profileSettings.ComplexTypeAttributeSpecializationTypeId
                        : profileSettings.AttributeSpecializationTypeId;

                    var complexTypeProperty = ElementPersistable.Create(
                        specializationType: complexPropSpec,
                        specializationTypeId: complexPropSpecId,
                        name: Casing(config, property.Name),
                        parentId: intentElement.Id,
                        externalReference: classification.Path);

                    complexTypeProperty.TypeReference = TypeReferencePersistable.Create(
                        typeId: targetElement.Id,
                        isNavigable: true,
                        isNullable: default,
                        isCollection: classification.IsCollection,
                        isRequired: default,
                        comment: classification.Remarks,
                        genericTypeId: default,
                        typePackageName: default,
                        typePackageId: default,
                        stereotypes: [],
                        genericTypeParameters: []);
                    
                    intentElement.ChildElements.Add(complexTypeProperty);
                }

                var (parsedClassElements, parsedAssociations) = Parse(
                    config: config,
                    jsonElement: classification.Element!.Value,
                    intentElement: targetElement,
                    path: classification.Path,
                    pathParts: pathParts.Append(Casing(config, property.Name)).ToArray(),
                    profileSettings: profileSettings,
                    typeLookups: typeLookups,
                    isRootElement: false,
                    parentFolderId: parentFolderId);
                
                classElements.AddRange(parsedClassElements);
                associations.AddRange(parsedAssociations);

                continue;
            }

            var referencedType = typeLookups[classification.Type];

            var attributeSpecializationType = intentElement.SpecializationTypeId == profileSettings.ComplexTypeSpecializationTypeId
                ? profileSettings.ComplexTypeAttributeSpecializationType
                : profileSettings.AttributeSpecializationType;
            var attributeSpecializationTypeId = intentElement.SpecializationTypeId == profileSettings.ComplexTypeSpecializationTypeId
                ? profileSettings.ComplexTypeAttributeSpecializationTypeId
                : profileSettings.AttributeSpecializationTypeId;
            
            var attributeElement = ElementPersistable.Create(
                specializationType: attributeSpecializationType,
                specializationTypeId: attributeSpecializationTypeId,
                name: Casing(config, property.Name),
                parentId: intentElement.Id,
                externalReference: classification.Path);

            attributeElement.TypeReference = TypeReferencePersistable.Create(
                typeId: referencedType.Id,
                isNavigable: true,
                isNullable: default,
                isCollection: classification.IsCollection,
                isRequired: default,
                comment: classification.Remarks,
                genericTypeId: default,
                typePackageName: referencedType.PackageName,
                typePackageId: referencedType.PackageId,
                stereotypes: [],
                genericTypeParameters: []);

            if (isRootElement &&
                "Id".Equals(attributeElement.Name, StringComparison.OrdinalIgnoreCase) &&
                config.Profile == ImportProfile.DomainDocumentDB &&
                profileSettings.AttributeSpecializationTypeId == AttributeModel.SpecializationTypeId)
            {
                attributeElement.Stereotypes.Add(new StereotypePersistable
                {
                    DefinitionId = "64f6a994-4909-4a9d-a0a9-afc5adf2ef74",
                    Name = "Primary Key",
                    Comment = null!,
                    AddedByDefault = false,
                    DefinitionPackageName = "Intent.Metadata.DocumentDB",
                    DefinitionPackageId = "1c7eab18-9482-4b4e-b61b-1fbd2d2427b6",
                    Properties = []
                });

                attributeElement.Metadata.Add(new GenericMetadataPersistable
                {
                    Key = "is-managed-key",
                    Value = "true"
                });
            }

            intentElement.ChildElements.Add(attributeElement);
        }

        return new(classElements, associations);
    }

    private static PropertyClassification Classify(JsonElement element, string path)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
            {
                return new PropertyClassification(
                    IsCollection: false,
                    Type: ClassificationType.Object,
                    Path: path,
                    Element: element);
            }
            case JsonValueKind.Array:
            {
                if (element.GetArrayLength() == 0)
                {
                    return new PropertyClassification(
                        IsCollection: true,
                        Type: ClassificationType.Unknown,
                        Element: null,
                        Path: path,
                        Remarks: "Array contained no elements so could not determine type.");
                }

                var firstItem = element.EnumerateArray().FirstOrDefault();
                if (firstItem.ValueKind == JsonValueKind.Array)
                {
                    return new PropertyClassification(
                        IsCollection: true,
                        Type: ClassificationType.Unknown,
                        Path: path,
                        Element: null,
                        Remarks: "Arrays of arrays are unsupported within the Intent domain designer.");
                }

                return Classify(firstItem, $"{path}[0]") with { IsCollection = true };
            }
            case JsonValueKind.String:
            {
                var stringValue = element.GetString();
                if (stringValue == "guid")
                {
                    return new PropertyClassification(
                        IsCollection: false,
                        Type: ClassificationType.Guid,
                        Path: path,
                        Element: element);
                }

                if (stringValue == "datetime")
                {
                    return new PropertyClassification(
                        IsCollection: false,
                        Type: ClassificationType.DateTime,
                        Path: path,
                        Element: element);
                }

                return new PropertyClassification(
                    IsCollection: false,
                    Type: ClassificationType.String,
                    Path: path,
                    Element: element);
            }
            case JsonValueKind.Number:
                return new PropertyClassification(
                    IsCollection: false,
                    Type: ClassificationType.Number,
                    Path: path,
                    Element: element);
            case JsonValueKind.True:
            case JsonValueKind.False:
                return new PropertyClassification(
                    IsCollection: false,
                    Type: ClassificationType.Bool,
                    Path: path,
                    Element: element);
            case JsonValueKind.Null:
            case JsonValueKind.Undefined:
                return new PropertyClassification(
                    IsCollection: false,
                    Type: ClassificationType.Unknown,
                    Path: path,
                    Element: null,
                    Remarks: $"Unable to determine type in JSON for {element.ValueKind} {nameof(JsonValueKind)}.");
            default:
                throw new ArgumentOutOfRangeException($"Cannot classify JSON element of type '{element.ValueKind}'");
        }
    }
    
    private static string CreateFolderHierarchy(
        string directoryPath,
        Dictionary<string, ElementPersistable> createdFolders,
        List<ClassElement> classElements)
    {
        // Normalize path separators
        directoryPath = directoryPath.Replace('\\', '/');

        // Split path into parts
        var pathParts = directoryPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
        string? parentFolderId = null;
        var currentPath = "";

        // Create folders for each part of the path
        for (int i = 0; i < pathParts.Length; i++)
        {
            currentPath = i == 0 ? pathParts[i] : $"{currentPath}/{pathParts[i]}";

            if (!createdFolders.TryGetValue(currentPath, out var folderElement))
            {
                // Create new folder element
                folderElement = ElementPersistable.Create(
                    specializationType: "Folder",
                    specializationTypeId: "4d95d53a-8855-4f35-aa82-e312643f5c5f",
                    name: pathParts[i],
                    parentId: parentFolderId,
                    externalReference: $"folder:{currentPath}");

                createdFolders[currentPath] = folderElement;
                classElements.Add(new (folderElement, new Stack<string>(pathParts.Take(i + 1).Reverse())));
            }

            parentFolderId = folderElement.Id;
        }

        return parentFolderId!;
    }

    private record PropertyClassification(
        bool IsCollection,
        ClassificationType Type,
        string Path,
        JsonElement? Element,
        string? Remarks = null);

    private enum ClassificationType
    {
        Object,
        Number,
        Bool,
        String,
        Guid,
        DateTime,
        Unknown
    }
}