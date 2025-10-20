using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.MetadataSynchronizer;
using Intent.Modules.Common.Templates;
using Intent.Modules.Json.Importer.Importer.Visitors;

namespace Intent.Modules.Json.Importer.Importer;

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
        // Resolve profile strategy
        var visitor = ProfileFactory.GetVisitorForProfile(config.Profile);
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

            var elementPersistable = visitor.VisitRoot(
                name: Utils.Casing(config, fileNameWithoutExtension),
                parentFolderId: parentFolderId,
                externalReference: relativePath.Replace('\\', '/'));

            classElements.Add(new(elementPersistable, new Stack<string>(new[] { fileNameWithoutExtension })));

            // Parse the document and add to classElements and associations
            var (parsedClassElements, parsedAssociations) = Parse(
                config: config,
                jsonElement: document.RootElement,
                intentElement: elementPersistable,
                path: fileNameWithoutExtension,
                pathParts: [Utils.Casing(config, fileNameWithoutExtension)],
                visitor: visitor,
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

    private record ParseResult(List<ClassElement> ClassElements, List<AssociationPersistable> Associations);
    private record ClassElement(ElementPersistable Element, Stack<string> PathParts);

    private static ParseResult Parse(
        JsonConfig config,
        JsonElement jsonElement,
        ElementPersistable intentElement,
        string path,
        IReadOnlyCollection<string> pathParts,
        IJsonElementVisitor visitor,
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
                var visitorResult = visitor.VisitObject(
                    config: config,
                    property: property,
                    owner: intentElement,
                    parentFolderId: parentFolderId,
                    sourcePath: path,
                    targetPath: classification.Path,
                    isCollection: classification.IsCollection);

                var targetElement = visitorResult.ClassElement;
                if (visitorResult.Association != null) associations.Add(visitorResult.Association);

                classElements.Add(new(targetElement, new Stack<string>(pathParts)));

                var (parsedClassElements, parsedAssociations) = Parse(
                    config: config,
                    jsonElement: classification.Element!.Value,
                    intentElement: targetElement,
                    path: classification.Path,
                    pathParts: pathParts.Append(Utils.Casing(config, property.Name)).ToArray(),
                    visitor: visitor,
                    typeLookups: typeLookups,
                    isRootElement: false,
                    parentFolderId: parentFolderId);

                classElements.AddRange(parsedClassElements);
                associations.AddRange(parsedAssociations);

                continue;
            }

            var referencedType = typeLookups[classification.Type];
            visitor.VisitProperty(
                config: config,
                property: property,
                owner: intentElement,
                externalReference: classification.Path,
                referencedType: referencedType,
                isCollection: classification.IsCollection,
                remarks: classification.Remarks,
                isRootElement: isRootElement);
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
                classElements.Add(new(folderElement, new Stack<string>(pathParts.Take(i + 1).Reverse())));
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
