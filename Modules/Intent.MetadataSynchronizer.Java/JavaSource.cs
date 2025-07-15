using Intent.Code.Weaving.Java.Editor;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.Types.Api;
using Serilog;
using System.Text.RegularExpressions;
using AttributeModel = Intent.Modelers.Domain.Api.AttributeModel;

namespace Intent.MetadataSynchronizer.Java
{
    internal static class JavaSource
    {
        public static Persistables GetPersistables(string sourcesPath,
            IReadOnlyCollection<PackageModelPersistable> packages,
            IReadOnlyCollection<string> ignores,
            bool applyJavaStereotypes)
        {
            var directoryInfo = new DirectoryInfo(sourcesPath);
            var toScan = GetJavaDirectories(null, directoryInfo).ToArray();
            if (!toScan.Any())
            {
                toScan = new[] { (default(string), directoryInfo) };
            }

            var ignorePatterns = ignores
                .Select(x => $"^{Regex.Escape(x).Replace(@"\*", ".*").Replace(@"\?", ".")}$")
                .ToArray();
            var persistables = toScan
                .Select(x => GetPersistables(
                    artifactName: x.Name,
                    rootDirectory: x.Directory,
                    packages: packages,
                    ignorePatterns: ignorePatterns,
                    applyJavaStereotypes: applyJavaStereotypes))
                .ToArray();

            return new Persistables(
                Elements: persistables.SelectMany(x => x.Elements).ToList(),
                Associations: persistables.SelectMany(x => x.Associations).ToList());

            // Can detect if the configured path contains multiple nested maven projects and then
            // results in them each being processed in isolation so that lookups by package work
            // properly and don't conflict. If no maven project is found, then we ultimately treat
            // it is a simple single source.
            static IEnumerable<(string Name, DirectoryInfo Directory)> GetJavaDirectories(string artifactName, DirectoryInfo directory)
            {
                if (directory.EnumerateFiles("pom.xml", SearchOption.TopDirectoryOnly).Any())
                {
                    Log.Debug(Indentation.Get() + "pom.xml file found at {Directory}", directory.FullName);
                    artifactName = directory.Name;
                }

                if (directory.Name.Equals("test", StringComparison.OrdinalIgnoreCase))
                {
                    Log.Debug(Indentation.Get() + "Test folder found at {Directory}, assuming is for unit tests and skipping.", directory.FullName);
                    yield break;
                }

                if (directory.Name.Equals("java", StringComparison.OrdinalIgnoreCase))
                {
                    Log.Debug(Indentation.Get() + "\"java\" folder found at {Directory}", directory.FullName);
                    yield return (artifactName, directory);
                    yield break;
                }

                foreach (var subDirectory in directory.GetDirectories().SelectMany(x => GetJavaDirectories(artifactName, x)))
                {
                    yield return subDirectory;
                }
            }
        }

        private static Persistables GetPersistables(string artifactName,
            DirectoryInfo rootDirectory,
            IReadOnlyCollection<PackageModelPersistable> packages,
            IReadOnlyCollection<string> ignorePatterns,
            bool applyJavaStereotypes)
        {
            Log.Debug(Indentation.Get() + "Getting persistables from {Folder} for {ArtifactName}", rootDirectory.FullName, artifactName);
            using var indentation = new Indentation();

            var lookups = new MetadataLookup(packages);
            var initialItems = Enumerable.Empty<IItem>();

            var parentFolder = default(ElementPersistable);
            if (artifactName != null)
            {
                parentFolder = ElementPersistable.Create(
                    specializationType: FolderModel.SpecializationType,
                    specializationTypeId: FolderModel.SpecializationTypeId,
                    name: artifactName,
                    parentId: null,
                    externalReference: artifactName);

                initialItems = initialItems.Append(new Folder(parentFolder));
            }

            var items = initialItems
                .Concat(GetElements(
                    rootDirectory: rootDirectory,
                    directory: rootDirectory,
                    folderElement: parentFolder,
                    ignorePatterns: ignorePatterns,
                    applyJavaStereotypes: applyJavaStereotypes))
                .ToArray();

            var byReference = items.ToDictionary(x => (x.Element.ExternalReference, x.GetType()));
            var byElement = items.ToDictionary(x => x.Element);
            lookups.AddElements(items.OfType<Class>().Select(x => x.Element));
            lookups.AddElements(items.OfType<Enum>().Select(x => x.Element));
            var associations = new List<AssociationPersistable>();
            var elements = new List<ElementPersistable>(items.Select(x => x.Element));

            foreach (var item in byReference.Values.OfType<Class>())
            {
                var (element, @class, _) = item;
                Log.Debug(Indentation.Get() + "Processing Class {Class}", @class.Name);
                using var indentation1 = new Indentation();

                // Fields
                foreach (var field in @class.Fields)
                {
                    Log.Debug(Indentation.Get() + "Processing Field {Field}", field.Name);
                    using var indentation2 = new Indentation();
                    var fieldReferencedType = GetTypeReference(field.Type, item, lookups);

                    if (!TryGetReferencedClass(lookups, fieldReferencedType.TypeReference, out var otherEndElement))
                    {
                        Log.Debug(Indentation.Get() + "Matched as attribute.");
                        var attribute = ElementPersistable.Create(
                            specializationType: AttributeModel.SpecializationType,
                            specializationTypeId: AttributeModel.SpecializationTypeId,
                            name: field.Name,
                            parentId: element.Id,
                            externalReference: $"{element.ExternalReference}.{field.Name}");
                        attribute.TypeReference = fieldReferencedType.TypeReference;
                        elements.Add(attribute);
                        continue;
                    }

                    if (lookups.GetAssociationsFor(element)
                        .Any(x => (x.TargetEnd.TypeReference.TypeId == element.Id && x.SourceEnd.ExternalReference == field.Name) ||
                                  (x.SourceEnd.TypeReference.TypeId == element.Id && x.TargetEnd.ExternalReference == field.Name)))
                    {
                        Log.Debug(Indentation.Get() + "Association already existed due to being created from its other end.");
                        continue;
                    }

                    var otherEndClass = (Class)byElement[otherEndElement];
                    var (otherEndField, otherEndTypeReference) = otherEndClass.Source.Fields
                        .Where(otherField => !otherField.Equals(field))
                        .Select(otherField => new
                        {
                            ReferencedType = GetTypeReference(otherField.Type, otherEndClass, lookups),
                            OtherEndField = otherField
                        })
                        .Where(x => TryGetReferencedClass(lookups, x.ReferencedType.TypeReference, out var otherElement) &&
                                    element.Id == otherElement.Id)
                        .Select(x => (x.OtherEndField, x.ReferencedType))
                        .FirstOrDefault();

                    Log.Debug(Indentation.Get() + "Making new association.");
                    var association = new AssociationPersistable
                    {
                        Id = Guid.NewGuid().ToString().ToLower(),
                        SourceEnd = new AssociationEndPersistable
                        {
                            SpecializationType = "Association Source End", // https://dev.azure.com/intentarchitect/Intent%20Architect/_workitems/edit/584
                            SpecializationTypeId = AssociationSourceEndModel.SpecializationTypeId,
                            Name = otherEndField?.Name,
                            TypeReference = TypeReferencePersistable.Create(
                                typeId: element.Id,
                                isNavigable: otherEndTypeReference != null,
                                isNullable: default,
                                isCollection: otherEndTypeReference?.ReferencedTypeIsCollection == true ||
                                              otherEndTypeReference?.TypeReference.IsCollection == true,
                                isRequired: default,
                                comment: default,
                                genericTypeId: default,
                                typePackageName: default,
                                typePackageId: default,
                                stereotypes: new List<StereotypePersistable>(),
                                genericTypeParameters: new List<TypeReferencePersistable>()),
                            ExternalReference = otherEndField?.Name,
                        },
                        TargetEnd = new AssociationEndPersistable
                        {
                            SpecializationType = "Association Target End", // https://dev.azure.com/intentarchitect/Intent%20Architect/_workitems/edit/584
                            SpecializationTypeId = AssociationTargetEndModel.SpecializationTypeId,
                            Name = field.Name,
                            TypeReference = TypeReferencePersistable.Create(
                                typeId: otherEndElement.Id,
                                isNavigable: true,
                                isNullable: default,
                                isCollection: fieldReferencedType.ReferencedTypeIsCollection ||
                                              fieldReferencedType.TypeReference?.IsCollection == true,
                                isRequired: default,
                                comment: default,
                                genericTypeId: default,
                                typePackageName: default,
                                typePackageId: default,
                                stereotypes: new List<StereotypePersistable>(),
                                genericTypeParameters: new List<TypeReferencePersistable>()),
                            ExternalReference = field.Name
                        },
                        AssociationType = AssociationModel.SpecializationType,
                        AssociationTypeId = AssociationModel.SpecializationTypeId
                    };

                    associations.Add(association);
                    lookups.AddAssociation(association);
                }
            }

            foreach (var element in elements)
            {
                if (element.ExternalReference == artifactName)
                {
                    continue;
                }

                element.ExternalReference = $"{artifactName}.{element.ExternalReference}";
            }

            foreach (var association in associations)
            {
                association.SourceEnd.ExternalReference = $"{artifactName}.{association.SourceEnd.ExternalReference}";
                association.TargetEnd.ExternalReference = $"{artifactName}.{association.TargetEnd.ExternalReference}";
            }

            return new Persistables(elements, associations);
        }

        private static IEnumerable<IItem> GetElements(FileSystemInfo rootDirectory,
            DirectoryInfo directory,
            IElementPersistable folderElement,
            IReadOnlyCollection<string> ignorePatterns,
            bool applyJavaStereotypes)
        {
            foreach (var subDirectory in directory.GetDirectories())
            {
                var subFolder = ElementPersistable.Create(
                    specializationType: FolderModel.SpecializationType,
                    specializationTypeId: FolderModel.SpecializationTypeId,
                    name: subDirectory.Name,
                    parentId: folderElement?.Id,
                    externalReference: Path.GetRelativePath(rootDirectory.FullName, subDirectory.FullName));

                yield return new Folder(subFolder);

                foreach (var model in GetElements(
                             rootDirectory: rootDirectory,
                             directory: subDirectory,
                             folderElement: subFolder,
                             ignorePatterns: ignorePatterns,
                             applyJavaStereotypes: applyJavaStereotypes))
                {
                    yield return model;
                }
            }

            foreach (var file in directory.EnumerateFiles("*.java"))
            {
                if (ignorePatterns.Any(pattern => Regex.IsMatch(
                        input: file.Name,
                        pattern: pattern,
                        options: RegexOptions.IgnoreCase)))
                {
                    continue;
                }

                Log.Debug(Indentation.Get() + "Reading {FullPath}", file.FullName);
                using var indentation = new Indentation();

                var javaFile = JavaFile.Parse(File.ReadAllText(file.FullName));
                var package = javaFile.Package.Name;

                foreach (var @class in javaFile.Classes)
                {
                    Log.Debug(Indentation.Get() + "Found class {Name}", @class.Name);
                    var element = ElementPersistable.Create(
                        specializationType: ClassModel.SpecializationType,
                        specializationTypeId: ClassModel.SpecializationTypeId,
                        name: @class.Name,
                        parentId: folderElement?.Id,
                        externalReference: string.IsNullOrWhiteSpace(package)
                            ? @class.Name
                            : $"{package}.{@class.Name}");

                    if (applyJavaStereotypes)
                    {
                        element.Stereotypes.Add(Stereotypes.Java.Create(package));
                    }

                    yield return new Class(element, @class, file.FullName);
                }

                foreach (var @enum in javaFile.Enums)
                {
                    Log.Debug(Indentation.Get() + "Found enum {Name}", @enum.Name);
                    var element = ElementPersistable.Create(
                        specializationType: EnumModel.SpecializationType,
                        specializationTypeId: EnumModel.SpecializationTypeId,
                        name: @enum.Name,
                        parentId: folderElement?.Id,
                        externalReference: string.IsNullOrWhiteSpace(package)
                            ? @enum.Name
                            : $"{package}.{@enum.Name}");

                    if (applyJavaStereotypes)
                    {
                        element.Stereotypes.Add(Stereotypes.Java.Create(package));
                    }

                    yield return new Enum(element);
                }
            }
        }

        private static bool TryGetReferencedClass(MetadataLookup metadataLookup, TypeReferencePersistable typeReference, out ElementPersistable element)
        {
            if (typeReference.TypeId == null)
            {
                element = default;
                return false;
            }

            if (metadataLookup.TryGetElementById(typeReference.TypeId, out element) &&
                element.SpecializationTypeId == ClassModel.SpecializationTypeId)
            {
                return true;
            }

            foreach (var genericTypeParameter in typeReference.GenericTypeParameters)
            {
                if (TryGetReferencedClass(metadataLookup, genericTypeParameter, out element))
                {
                    return true;
                }
            }

            element = default;
            return false;
        }

        private record ReferencedType(TypeReferencePersistable TypeReference, bool ReferencedTypeIsCollection);

        private static ReferencedType GetTypeReference(
            JavaType type,
            Class @class,
            MetadataLookup metadataLookup)
        {
            return GetTypeDefinitionLocal(
                typeLocal: type,
                @class: @class,
                lookups: metadataLookup,
                typeQualifier: !string.IsNullOrWhiteSpace(@class.Source.File.Package.Name)
                    ? $"{@class.Source.File.Package.Name}."
                    : string.Empty,
                genericTypeId: null);

            static ReferencedType GetTypeDefinitionLocal(
                JavaType typeLocal,
                Class @class,
                MetadataLookup lookups,
                string typeQualifier,
                string genericTypeId)
            {
                var arrayDepth = 0;
                var typeName = typeLocal.Name.TrimEnd();

                while (typeName.EndsWith("[]"))
                {
                    arrayDepth++;
                    typeName = typeName[..^"[]".Length];
                }

                if (!TryGetReferencedType(typeName, typeLocal.GenericTypeParameters.Count, out var referencedType))
                {
                    var unresolvedType = TryGetQualifiedTypeName(@class.Source.File, typeName, @class.FilePath,
                        out var qualifiedTypeName)
                        ? qualifiedTypeName
                        : typeLocal.ToString();

                    Log.Warning(Indentation.Get() + "Could not resolve type {TypeComponent} of {FullType} in {FilePath}", typeLocal, typeLocal, @class.FilePath);
                    return new ReferencedType(
                        TypeReference: new TypeReferencePersistable
                        {
                            Comment = $"Could not resolve type for:{Environment.NewLine}{unresolvedType}"
                        },
                        ReferencedTypeIsCollection: false);
                }

                var genericTypeParameters = referencedType!.GenericTypes
                    .Zip(typeLocal.GenericTypeParameters)
                    .Select(x =>
                    {
                        var (genericType, javaType) = x;
                        var referencedType = GetTypeDefinitionLocal(
                            typeLocal: javaType,
                            @class: @class,
                            lookups: lookups,
                            typeQualifier: typeQualifier,
                            genericTypeId: genericType.Id);

                        return referencedType.TypeReference;
                    })
                    .ToList();

                var referencedTypeIsCollection = referencedType.Stereotypes
                    .Any(x => x.Name == Stereotypes.Java.Name && x.Properties
                        .Any(y => y.Name == Stereotypes.Java.Property.IsCollection && y.Value == "true"));

                return new ReferencedType(
                    TypeReference: TypeReferencePersistable.Create(
                        typeId: referencedType.Id,
                        isNavigable: true,
                        isNullable: default,
                        isCollection: arrayDepth > 0,
                        isRequired: default,
                        comment: default,
                        genericTypeId: genericTypeId,
                        typePackageName: referencedType.PackageName,
                        typePackageId: referencedType.PackageId,
                        stereotypes: new List<StereotypePersistable>(),
                        genericTypeParameters: genericTypeParameters),
                    ReferencedTypeIsCollection: referencedTypeIsCollection);

                bool TryGetReferencedType(string typeName, int genericTypeParameterCount, out ElementPersistable element)
                {
                    element = default;
                    typeName = typeName == "boolean"
                        ? "bool"
                        : typeName;

                    if (lookups.TryGetTypeDefinitionByName(typeName, genericTypeParameterCount, out element))
                    {
                        return true;
                    }

                    if (typeLocal.GenericTypeParameters.Count == 0 && (
                            (TryGetQualifiedTypeName(@class.Source.File, typeName, @class.FilePath, out var qualifiedTypeName) &&
                             (
                                 lookups.TryGetEnumByReference(qualifiedTypeName, out element) ||
                                 lookups.TryGetClassByReference(qualifiedTypeName, out element)
                             )) ||
                            lookups.TryGetEnumByReference($"{typeQualifier}{typeName}", out element) ||
                            lookups.TryGetClassByReference($"{typeQualifier}{typeName}", out element)
                        ))
                    {
                        return true;
                    }

                    return false;
                }
            }
        }

        private static bool TryGetQualifiedTypeName(
            JavaFile javaFile,
            string type,
            string sourceFilePath,
            out string fullyQualifiedType)
        {
            fullyQualifiedType = type;
            var import = javaFile.Imports
                .Select(x => x.TypeName)
                .Where(x => x.EndsWith($".{type}"))
                .ToArray();

            switch (import.Length)
            {
                case 0:
                    {
                        return false;
                    }
                case 1:
                    fullyQualifiedType = import[0];
                    return true;
                default:
                    Log.Warning(Indentation.Get() + "More than single matching import for {Type} in {File}", type, sourceFilePath);
                    return false;
            }
        }

        private record Class(ElementPersistable Element, JavaClass Source, string FilePath) : IItem;

        private record Enum(ElementPersistable Element) : IItem;

        private record Folder(ElementPersistable Element) : IItem;

        private interface IItem
        {
            public ElementPersistable Element { get; }
        }
    }
}
