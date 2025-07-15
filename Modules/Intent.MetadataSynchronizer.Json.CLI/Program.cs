using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.MetadataSynchronizer.Configuration;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.Templates;

namespace Intent.MetadataSynchronizer.Json.CLI
{
    internal class Program
    {
        private static string GetOptionName(string propertyName) => $"--{propertyName.ToKebabCase()}";

        static void Main(string[] args)
        {
            var rootCommand = new RootCommand(
                "The Intent JSON Metadata Synchronizer CLI tool can be used to synchronize an object " +
                "graph in a JSON file into an Intent Architect Document DB Domain Package.")
            {
                new Option<FileInfo>(
                    name: GetOptionName(nameof(JsonConfig.ConfigFile)),
                    description: "Path to a JSON formatted file containing options to use for execution of " +
                                 "this tool as an alternative to using command line options. The " +
                                 $"{GetOptionName(nameof(JsonConfig.GenerateConfigFile))} option can be used to " +
                                 "generate a file with all the possible fields populated with null."),
                new Option<bool>(
                    name: GetOptionName(nameof(JsonConfig.GenerateConfigFile)),
                    description: $"Scaffolds into the current working directory a \"config.json\" for use with the " +
                                 $"{GetOptionName(nameof(JsonConfig.ConfigFile))} option."),
                new Option<FileInfo>(
                    name: GetOptionName(nameof(JsonConfig.SourceJsonFile)),
                    description: "The name of the JSON file to parse and synchronize into the Intent Architect Package."),
                new Option<FileInfo>(
                    name: GetOptionName(nameof(JsonConfig.IslnFile)),
                    description: "The Intent Architect solution (.isln) file containing the Intent Architect Application " +
                                 "into which to synchronize the metadata."),
                new Option<string>(
                    name: GetOptionName(nameof(JsonConfig.ApplicationName)),
                    description: "The name of the Intent Architect Application (as per the Application Settings view) " +
                                 "containing the Intent Architect Package into which to synchronize the metadata."),
                new Option<string>(
                    name: GetOptionName(nameof(JsonConfig.PackageId)),
                    description: "The id of the Intent Architect Package containing the Intent Architect Package into " +
                                 "which to synchronize the metadata."),
                new Option<string>(
                    name: GetOptionName(nameof(JsonConfig.TargetFolderId)),
                    description: "The target folder within the Intent Architect package into which to synchronize the " +
                                 "metadata. If unspecified then the metadata will be synchronized into the root of the " +
                                 "Intent Architect package."),
                new Option<CasingConvention?>(
                    name: GetOptionName(nameof(JsonConfig.CasingConvention)),
                    description: "Casing convention to be applied on imported elements. Options are PascalCase or AsIs.")
            };

            rootCommand.SetHandler(
                handle: (
                    FileInfo? configFilePath,
                    bool generateConfigFile,
                    FileInfo? sourceJsonFile,
                    FileInfo? islnFile,
                    string? applicationName,
                    string? packageId,
                    string? targetFolderId,
                    CasingConvention? casingConvention) =>
                {
                    var serializerOptions = new JsonSerializerOptions
                    {
                        Converters = { new JsonStringEnumConverter() },
                        WriteIndented = true
                    };

                    if (generateConfigFile)
                    {
                        var path = Path.Join(Environment.CurrentDirectory, "config.json");
                        Console.WriteLine($"Writing {path}...");
                        File.WriteAllBytes(path, JsonSerializer.SerializeToUtf8Bytes(new JsonConfig(), serializerOptions));
                        Console.WriteLine("Done.");
                        return;
                    }

                    var deleteExtra = false;
                    var debug = true;
                    var createAttributesWithUnknownTypes = true;
                    var stereotypeManagementMode = StereotypeManagementMode.Merge;

                    var configFileOptionName = GetOptionName(nameof(JsonConfig.ConfigFile));

                    JsonConfig configFile;
                    if (configFilePath != null)
                    {
                        if (sourceJsonFile != null)
                            throw new Exception($"{GetOptionName(nameof(JsonConfig.SourceJsonFile))} not allowed when {configFileOptionName} specified.");

                        if (islnFile != null)
                            throw new Exception($"{GetOptionName(nameof(JsonConfig.IslnFile))} not allowed when {configFileOptionName} specified.");

                        if (!string.IsNullOrWhiteSpace(applicationName))
                            throw new Exception($"{GetOptionName(nameof(JsonConfig.ApplicationName))} not allowed when {configFileOptionName} specified.");

                        if (!string.IsNullOrWhiteSpace(packageId))
                            throw new Exception($"{GetOptionName(nameof(JsonConfig.PackageId))} not allowed when {configFileOptionName} specified.");

                        if (!string.IsNullOrWhiteSpace(targetFolderId))
                            throw new Exception($"{GetOptionName(nameof(JsonConfig.TargetFolderId))} not allowed when {configFileOptionName} specified.");

                        configFile = JsonSerializer.Deserialize<JsonConfig>(File.ReadAllText(configFilePath.FullName), serializerOptions)
                                     ?? throw new Exception($"Parsing of \"{configFilePath.FullName}\" returned null.");

                        if (string.IsNullOrWhiteSpace(configFile.SourceJsonFile) || !File.Exists(configFile.SourceJsonFile))
                            throw new Exception($"Config file field {nameof(configFile.SourceJsonFile)} must be a valid file path.");

                        if (string.IsNullOrWhiteSpace(configFile.IslnFile) || !File.Exists(configFile.IslnFile))
                            throw new Exception($"Config file field {nameof(configFile.IslnFile)} must be a valid file path.");

                        if (string.IsNullOrWhiteSpace(configFile.ApplicationName))
                            throw new Exception($"Config file field {nameof(configFile.ApplicationName)} must be specified.");

                        if (string.IsNullOrWhiteSpace(configFile.PackageId))
                            throw new Exception($"Config file field {nameof(configFile.PackageId)} must be specified.");
                    }
                    else
                    {
                        if (sourceJsonFile == null)
                            throw new Exception($"{GetOptionName(nameof(JsonConfig.SourceJsonFile))} mandatory when {configFileOptionName} not specified.");

                        if (islnFile == null)
                            throw new Exception($"{GetOptionName(nameof(JsonConfig.IslnFile))} mandatory when {configFileOptionName} not specified.");

                        if (string.IsNullOrWhiteSpace(applicationName))
                            throw new Exception($"{GetOptionName(nameof(JsonConfig.ApplicationName))} mandatory when {configFileOptionName} not specified.");

                        if (string.IsNullOrWhiteSpace(packageId))
                            throw new Exception($"{GetOptionName(nameof(JsonConfig.PackageId))} mandatory when {configFileOptionName} not specified.");

                        configFile = new JsonConfig
                        {
                            SourceJsonFile = sourceJsonFile.FullName,
                            IslnFile = islnFile.FullName,
                            ApplicationName = applicationName,
                            PackageId = packageId,
                            TargetFolderId = !string.IsNullOrWhiteSpace(targetFolderId) ? targetFolderId : null
                        };
                        if (casingConvention != null)
                        {
                            configFile.CasingConvention = casingConvention.Value;
                        }
                    }

                    Helpers.Execute(
                        intentSolutionPath: configFile.IslnFile,
                        applicationName: configFile.ApplicationName,
                        designerName: "Domain",
                        packageId: configFile.PackageId,
                        targetFolderId: configFile.TargetFolderId,
                        deleteExtra: deleteExtra,
                        debug: debug,
                        createAttributesWithUnknownTypes: createAttributesWithUnknownTypes,
                        stereotypeManagementMode: stereotypeManagementMode,
                        additionalPreconditionChecks: null,
                        getPersistables: packages => GetPersistables(configFile.SourceJsonFile, configFile.CasingConvention, packages));
                },
                symbols: Enumerable.Empty<IValueDescriptor>()
                    .Concat(rootCommand.Options)
                    .ToArray());

            Console.WriteLine($"{rootCommand.Name} version {Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion}");

            new CommandLineBuilder(rootCommand)
                .UseDefaults()
                .Build()
                .Invoke(args);
        }

        private static Persistables GetPersistables(
            string sourceFile,
            CasingConvention casingConvention,
            IReadOnlyCollection<PackageModelPersistable> packages)
        {
            var lookups = new MetadataLookup(packages);
            if (!lookups.TryGetTypeDefinitionByName("bool", 0, out var boolType)) throw new Exception();
            if (!lookups.TryGetTypeDefinitionByName("object", 0, out var objectType)) throw new Exception();
            if (!lookups.TryGetTypeDefinitionByName("string", 0, out var stringType)) throw new Exception();
            if (!lookups.TryGetTypeDefinitionByName("decimal", 0, out var numberType)) throw new Exception();

            var typeLookups = new Dictionary<ClassificationType, ElementPersistable>
            {
                [ClassificationType.Bool] = boolType,
                [ClassificationType.String] = stringType,
                [ClassificationType.Object] = objectType,
                [ClassificationType.Unknown] = objectType,
                [ClassificationType.Number] = numberType,
            };

            var document = JsonDocument.Parse(File.ReadAllText(sourceFile), new JsonDocumentOptions
            {
                CommentHandling = JsonCommentHandling.Skip
            });

            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(sourceFile);
            
            var elementPersistable = ElementPersistable.Create(
                specializationType: ClassModel.SpecializationType,
                specializationTypeId: ClassModel.SpecializationTypeId,
                name: Casing(fileNameWithoutExtension),
                parentId: null,
                externalReference: fileNameWithoutExtension);

            var classElements = new List<(ElementPersistable Element, Stack<string> PathParts)>();
            var associations = new List<AssociationPersistable>();

            classElements.Add((elementPersistable, new Stack<string>(new[] { fileNameWithoutExtension })));

            Parse(
                jsonElement: document.RootElement, 
                intentElement: elementPersistable, 
                path: $"({fileNameWithoutExtension})",
                pathParts: new[] { Casing(fileNameWithoutExtension) },
                isRootElement: true);

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

            string Casing(string name)
            {
                return casingConvention == CasingConvention.AsIs ? name : name.ToPascalCase();
            }

            void Parse(
                JsonElement jsonElement,
                ElementPersistable intentElement,
                string path,
                IReadOnlyCollection<string> pathParts,
                bool isRootElement = false)
            {
                foreach (var property in jsonElement.EnumerateObject())
                {
                    var classification = Classify(property.Value, $"{path}.{property.Name}");
                    if (classification.Type == ClassificationType.Object)
                    {
                        var targetElement = ElementPersistable.Create(
                            specializationType: ClassModel.SpecializationType,
                            specializationTypeId: ClassModel.SpecializationTypeId,
                            name: Casing(property.Name).Singularize(false),
                            parentId: null,
                            externalReference: classification.Path);

                        classElements.Add((targetElement, new Stack<string>(pathParts)));

                        var association = new AssociationPersistable
                        {
                            Id = Guid.NewGuid().ToString().ToLower(),
                            SourceEnd = new AssociationEndPersistable
                            {
                                SpecializationType = "Association Source End", // https://dev.azure.com/intentarchitect/Intent%20Architect/_workitems/edit/584
                                SpecializationTypeId = AssociationSourceEndModel.SpecializationTypeId,
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
                                SpecializationType = "Association Target End", // https://dev.azure.com/intentarchitect/Intent%20Architect/_workitems/edit/584
                                SpecializationTypeId = AssociationTargetEndModel.SpecializationTypeId,
                                Name = Casing(property.Name),
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
                            AssociationType = AssociationModel.SpecializationType,
                            AssociationTypeId = AssociationModel.SpecializationTypeId
                        };

                        associations.Add(association);

                        Parse(classification.Element!.Value, targetElement, classification.Path, pathParts.Append(Casing(property.Name)).ToArray());

                        continue;
                    }

                    var referencedType = typeLookups[classification.Type];

                    var attributeElement = ElementPersistable.Create(
                        specializationType: AttributeModel.SpecializationType,
                        specializationTypeId: AttributeModel.SpecializationTypeId,
                        name: Casing(property.Name),
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
                        stereotypes: new List<StereotypePersistable>(),
                        genericTypeParameters: new List<TypeReferencePersistable>());

                    if (isRootElement &&
                        "Id".Equals(attributeElement.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        attributeElement.Stereotypes.Add(new StereotypePersistable
                        {
                            DefinitionId = "64f6a994-4909-4a9d-a0a9-afc5adf2ef74",
                            Name = "Primary Key",
                            Comment = null,
                            AddedByDefault = false,
                            DefinitionPackageName = "Intent.Metadata.DocumentDB",
                            DefinitionPackageId = "1c7eab18-9482-4b4e-b61b-1fbd2d2427b6",
                            Properties = new List<StereotypePropertyPersistable>()
                        });

                        attributeElement.Metadata.Add(new GenericMetadataPersistable
                        {
                            Key = "is-managed-key",
                            Value = "true"
                        });
                    }

                    intentElement.ChildElements.Add(attributeElement);
                }
            }

            PropertyClassification Classify(JsonElement element, string path)
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
                        return new PropertyClassification(
                            IsCollection: false,
                            Type: ClassificationType.String,
                            Path: path,
                            Element: element);
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
                        throw new ArgumentOutOfRangeException();
                }
            }
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
            Unknown
        }
    }
}