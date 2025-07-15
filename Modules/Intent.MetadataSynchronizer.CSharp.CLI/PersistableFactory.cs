using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.MetadataSynchronizer.CSharp.CLI.Builders;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;

namespace Intent.MetadataSynchronizer.CSharp.CLI;

internal static class PersistableFactory
{
    public static Persistables GetPersistables(CSharpConfig csConfig, CoreTypesData coreTypeElements,
        IReadOnlyCollection<PackageModelPersistable> packages)
    {
        if (!string.IsNullOrWhiteSpace(csConfig.TargetFolderId) && !packages.Any(s => s.GetElementById(csConfig.TargetFolderId) is not null))
        {
            throw new InvalidOperationException($"{nameof(csConfig.TargetFolderId)} has value of '{csConfig.TargetFolderId}' which could not be found.");
        }

        var elements = new List<ElementPersistable>();
        var associations = new List<AssociationPersistable>();

        var standardLookup = new MetadataLookup(packages);
        var classDataLookup = coreTypeElements.DomainClasses.ToDictionary(classData => $"{classData.Namespace}.{classData.Name}");

        var builderMetadataManager = new BuilderMetadataManager(standardLookup, elements);

        var classDataAndBuilders = RegisterDomainClasses(csConfig, coreTypeElements, builderMetadataManager, standardLookup, elements);
        RegisterDomainEnums(csConfig, coreTypeElements, builderMetadataManager, elements, standardLookup);
        var valueObjectAndBuilders = RegisterValueObjects(csConfig, coreTypeElements, builderMetadataManager, elements, standardLookup);
        var dataContractAndBuilders = RegisterDomainDataContracts(csConfig, coreTypeElements, builderMetadataManager, elements, standardLookup);

        var domainServicesAndBuilders = RegisterDomainServices(csConfig, coreTypeElements, builderMetadataManager, elements, standardLookup);
        var domainRepositoriesAndBuilders = RegisterDomainRepositories(csConfig, coreTypeElements, builderMetadataManager, elements, standardLookup);
        var serviceDtosAndBuilders = RegisterServiceDtos(csConfig, coreTypeElements, builderMetadataManager, elements, standardLookup);
        var eventMessagesAndBuilders = RegisterEventMessages(csConfig, coreTypeElements, builderMetadataManager, elements, standardLookup);
        var eventDtosAndBuilders = RegisterEventDtos(csConfig, coreTypeElements, builderMetadataManager, elements, standardLookup);

        RegisterServiceEnums(csConfig, coreTypeElements, builderMetadataManager, elements, standardLookup);

        PostProcessDomainClasses(classDataAndBuilders, standardLookup, elements, classDataLookup, builderMetadataManager, associations);
        PostProcessValueObjects(valueObjectAndBuilders, elements);
        PostProcessDataContracts(dataContractAndBuilders, elements);
        PostProcessDomainServices(domainServicesAndBuilders, elements);
        PostProcessRepositories(domainRepositoriesAndBuilders, elements);
        PostProcessDtos(serviceDtosAndBuilders, standardLookup, elements);
        PostProcessEventMessages(eventMessagesAndBuilders, standardLookup, elements);
        PostProcessEventDtos(eventDtosAndBuilders, standardLookup, elements);

        return new Persistables(elements, associations);
    }

    private static List<(ClassData, ClassElementBuilder)> RegisterDomainClasses(CSharpConfig csConfig, CoreTypesData coreTypeElements, BuilderMetadataManager builderMetadataManager,
        MetadataLookup standardLookup, List<ElementPersistable> elements)
    {
        var classDataAndBuilders = new List<(ClassData, ClassElementBuilder)>();

        foreach (var classData in coreTypeElements.DomainClasses)
        {
            var classBuilder = Builder.CreateClassBuilder(
                externalReference: $"{classData.Namespace}.{classData.Name}",
                name: classData.Name,
                builderMetadataManager: builderMetadataManager);
            classDataAndBuilders.Add((classData, classBuilder));

            var folders = GetFolderElements(
                folderPath: GetRelativeLocation(classData.FilePath, csConfig.DomainEntitiesFolder!),
                lookup: standardLookup,
                targetFolderId: csConfig.TargetFolderId);
            classBuilder.SetParentId(folders.LastOrDefault()?.Id);
            elements.AddRange(standardLookup.AddElementsIfMissing(folders));
        }

        return classDataAndBuilders;
    }

    private static void RegisterDomainEnums(CSharpConfig csConfig, CoreTypesData coreTypeElements, BuilderMetadataManager builderMetadataManager, List<ElementPersistable> elements,
        MetadataLookup standardLookup)
    {
        foreach (var enumData in coreTypeElements.DomainEnums)
        {
            RegisterCommonEnum(csConfig.DomainEnumsFolder!, csConfig.TargetFolderId, builderMetadataManager, elements, standardLookup, enumData);
        }
    }

    private static List<(ClassData, DomainServiceElementBuilder)> RegisterDomainServices(CSharpConfig csConfig, CoreTypesData coreTypeElements, BuilderMetadataManager builderMetadataManager,
        List<ElementPersistable> elements, MetadataLookup standardLookup)
    {
        var domainServicesAndBuilders = new List<(ClassData, DomainServiceElementBuilder)>();

        foreach (var domainServiceData in coreTypeElements.DomainServices)
        {
            var domainServiceBuilder = Builder.CreateDomainServiceBuilder(
                externalReference: $"{domainServiceData.Namespace}.{domainServiceData.Name}",
                name: domainServiceData.Name,
                builderMetadataManager: builderMetadataManager);

            domainServicesAndBuilders.Add((domainServiceData, domainServiceBuilder));

            var folders = GetFolderElements(
                folderPath: GetRelativeLocation(domainServiceData.FilePath, csConfig.DomainServicesFolder!),
                lookup: standardLookup,
                targetFolderId: csConfig.TargetFolderId);
            domainServiceBuilder.SetParentId(folders.LastOrDefault()?.Id);
            elements.AddRange(standardLookup.AddElementsIfMissing(folders));
        }

        return domainServicesAndBuilders;
    }

    private static List<(ClassData, DomainRepositoryBuilder)> RegisterDomainRepositories(CSharpConfig csConfig, CoreTypesData coreTypeElements, BuilderMetadataManager builderMetadataManager,
        List<ElementPersistable> elements, MetadataLookup standardLookup)
    {
        var domainRepositoriesAndBuilders = new List<(ClassData, DomainRepositoryBuilder)>();

        foreach (var domainRepositoryData in coreTypeElements.DomainRepositories)
        {
            var domainRepositoryBuilder = Builder.CreateDomainRepositoryBuilder(
                externalReference: $"{domainRepositoryData.Namespace}.{domainRepositoryData.Name}",
                name: domainRepositoryData.Name,
                builderMetadataManager: builderMetadataManager);

            domainRepositoriesAndBuilders.Add((domainRepositoryData, domainRepositoryBuilder));

            var folders = GetFolderElements(
                folderPath: GetRelativeLocation(domainRepositoryData.FilePath, csConfig.DomainRepositoriesFolder!),
                lookup: standardLookup,
                targetFolderId: csConfig.TargetFolderId);
            domainRepositoryBuilder.SetParentId(folders.LastOrDefault()?.Id);
            elements.AddRange(standardLookup.AddElementsIfMissing(folders));
        }

        return domainRepositoriesAndBuilders;
    }

    private static bool WithoutCancellationToken(ParameterData paramData)
    {
        return paramData.Type?.Contains("CancellationToken") == false;
    }

    private static List<(ClassData, ValueObjectBuilder)> RegisterValueObjects(CSharpConfig csConfig, CoreTypesData coreTypeElements, BuilderMetadataManager builderMetadataManager,
        List<ElementPersistable> elements, MetadataLookup standardLookup)
    {
        var valueObjectAndBuilders = new List<(ClassData, ValueObjectBuilder)>();

        foreach (var valueObjectData in coreTypeElements.ValueObjects)
        {
            var valueObjectBuilder = Builder.CreateValueObjectBuilder(
                externalReference: $"{valueObjectData.Namespace}.{valueObjectData.Name}",
                name: valueObjectData.Name,
                builderMetadataManager: builderMetadataManager);
            valueObjectAndBuilders.Add((valueObjectData, valueObjectBuilder));

            var folders = GetFolderElements(
                folderPath: GetRelativeLocation(valueObjectData.FilePath, csConfig.ValueObjectsFolder!),
                lookup: standardLookup,
                targetFolderId: csConfig.TargetFolderId);
            valueObjectBuilder.SetParentId(folders.LastOrDefault()?.Id);
            elements.AddRange(standardLookup.AddElementsIfMissing(folders));
        }

        return valueObjectAndBuilders;
    }

    private static List<(ClassData, DomainDataContractBuilder)> RegisterDomainDataContracts(CSharpConfig csConfig, CoreTypesData coreTypeElements, BuilderMetadataManager builderMetadataManager,
        List<ElementPersistable> elements, MetadataLookup standardLookup)
    {
        var dataContractAndBuilders = new List<(ClassData, DomainDataContractBuilder)>();

        foreach (var domainDataContractData in coreTypeElements.DomainDataContracts)
        {
            var domainDataContractBuilder = Builder.CreateDomainDataContractBuilder(
                externalReference: $"{domainDataContractData.Namespace}.{domainDataContractData.Name}",
                name: domainDataContractData.Name,
                builderMetadataManager: builderMetadataManager);
            dataContractAndBuilders.Add((domainDataContractData, domainDataContractBuilder));

            var folders = GetFolderElements(
                folderPath: GetRelativeLocation(domainDataContractData.FilePath, csConfig.DomainDataContractsFolder!),
                lookup: standardLookup,
                targetFolderId: csConfig.TargetFolderId);
            domainDataContractBuilder.SetParentId(folders.LastOrDefault()?.Id);
            elements.AddRange(standardLookup.AddElementsIfMissing(folders));
        }

        return dataContractAndBuilders;
    }

    private static void RegisterServiceEnums(CSharpConfig csConfig, CoreTypesData coreTypeElements, BuilderMetadataManager builderMetadataManager,
        List<ElementPersistable> elements, MetadataLookup standardLookup)
    {
        foreach (var enumData in coreTypeElements.ServiceEnums)
        {
            RegisterCommonEnum(csConfig.ServiceEnumsFolder!, csConfig.TargetFolderId, builderMetadataManager, elements, standardLookup, enumData);
        }
    }

    private static List<(ClassData, DtoElementBuilder)> RegisterServiceDtos(CSharpConfig csConfig, CoreTypesData coreTypeElements, BuilderMetadataManager builderMetadataManager,
        List<ElementPersistable> elements, MetadataLookup standardLookup)
    {
        var dtoAndBuilders = new List<(ClassData, DtoElementBuilder)>();
        foreach (var serviceDtoData in coreTypeElements.ServiceDTOs)
        {
            var dtoBuilder = Builder.CreateServiceDtoBuilder(
                externalReference: $"{serviceDtoData.Namespace}.{serviceDtoData.Name}",
                name: serviceDtoData.Name,
                builderMetadataManager: builderMetadataManager);

            dtoAndBuilders.Add((serviceDtoData, dtoBuilder));

            var folders = GetFolderElements(
                folderPath: GetRelativeLocation(serviceDtoData.FilePath, csConfig.ServiceDtosFolder!),
                lookup: standardLookup,
                targetFolderId: csConfig.TargetFolderId);
            dtoBuilder.SetParentId(folders.LastOrDefault()?.Id);
            elements.AddRange(standardLookup.AddElementsIfMissing(folders));
        }

        return dtoAndBuilders;
    }

    private static List<(ClassData, EventMessageElementBuilder)> RegisterEventMessages(CSharpConfig csConfig, CoreTypesData coreTypeElements, BuilderMetadataManager builderMetadataManager,
        List<ElementPersistable> elements, MetadataLookup standardLookup)
    {
        var eventMessageAndBuilders = new List<(ClassData, EventMessageElementBuilder)>();
        var coreTypes = coreTypeElements.EventMessages.Select(e => $"{e.Namespace}.{e.Name}").ToList();
        var eventingDtos = GetEventingDtos(coreTypeElements, coreTypes);

        foreach (var eventMessageData in coreTypeElements.EventMessages)
        {
            // only add it if its not a DTO
            if (!eventingDtos.Contains($"{eventMessageData.Namespace}.{eventMessageData.Name}"))
            {
                var messageBuilder = Builder.CreateEventMessageBuilder(
                externalReference: $"{eventMessageData.Namespace}.{eventMessageData.Name}",
                name: eventMessageData.Name,
                builderMetadataManager: builderMetadataManager);

                eventMessageAndBuilders.Add((eventMessageData, messageBuilder));

                var folders = GetFolderElements(
                    folderPath: GetRelativeLocation(eventMessageData.FilePath, csConfig.EventMessagesFolder!),
                    lookup: standardLookup,
                    targetFolderId: csConfig.TargetFolderId);
                messageBuilder.SetParentId(folders.LastOrDefault()?.Id);
                elements.AddRange(standardLookup.AddElementsIfMissing(folders));
            }
        }

        return eventMessageAndBuilders;
    }

    private static List<(ClassData, EventDtoElementBuilder)> RegisterEventDtos(CSharpConfig csConfig, CoreTypesData coreTypeElements, BuilderMetadataManager builderMetadataManager,
        List<ElementPersistable> elements, MetadataLookup standardLookup)
    {
        var eventDtosAndBuilders = new List<(ClassData, EventDtoElementBuilder)>();
        var coreTypes = coreTypeElements.EventMessages.Select(e => $"{e.Namespace}.{e.Name}").ToList();
        var eventingDtos = GetEventingDtos(coreTypeElements, coreTypes);

        foreach (var eventMessageData in coreTypeElements.EventMessages)
        {
            // only add it if its not a DTO
            if (eventingDtos.Contains($"{eventMessageData.Namespace}.{eventMessageData.Name}"))
            {
                var dtoBuilder = Builder.CreateEventDtoBuilder(
                externalReference: $"{eventMessageData.Namespace}.{eventMessageData.Name}",
                name: eventMessageData.Name,
                builderMetadataManager: builderMetadataManager);

                eventDtosAndBuilders.Add((eventMessageData, dtoBuilder));

                var folders = GetFolderElements(
                    folderPath: GetRelativeLocation(eventMessageData.FilePath, csConfig.EventMessagesFolder!),
                    lookup: standardLookup,
                    targetFolderId: csConfig.TargetFolderId);
                dtoBuilder.SetParentId(folders.LastOrDefault()?.Id);
                elements.AddRange(standardLookup.AddElementsIfMissing(folders));
            }
        }

        return eventDtosAndBuilders;
    }

    // this method will look through all the properties in the event messages, and check if any of them are of a type being imported
    // if so, then they are considered a DTO and not a Message
    private static List<string> GetEventingDtos(CoreTypesData coreTypeElements, List<string> coreTypes)
    {
        List<string> eventingDtos = new List<string>();

        foreach (var eventMessageData in coreTypeElements.EventMessages)
        {
            foreach (var prop in eventMessageData.Properties.Where(p => coreTypes.Contains(p.Type)))
            {
                eventingDtos.Add(prop.Type);
            }
        }

        return eventingDtos;
    }

    private static void PostProcessDomainClasses(List<(ClassData, ClassElementBuilder)> classDataAndBuilders, MetadataLookup standardLookup,
        List<ElementPersistable> elements, Dictionary<string, ClassData> classDataLookup,
        BuilderMetadataManager builderMetadataManager, List<AssociationPersistable> associations)
    {
        // handle associations first, so we can get a list of foreign keys to be used just below
        foreach (var classData in classDataLookup.Values)
        {
            var currentClassProperties = classData.Properties.Where(p => IsDomainType(p.Type!, standardLookup)).ToArray();
            foreach (var property in currentClassProperties)
            {
                if (!classDataLookup.TryGetValue(property.Type!, out var otherClass))
                {
                    continue;
                }

                // Is this a bi-directional association?
                var bidirectionalProperty = otherClass.Properties
                    .FirstOrDefault(p => IsDomainType(p.Type!, standardLookup) &&
                                         classDataLookup.TryGetValue(property.Type!, out var thisClass) &&
                                         thisClass == otherClass && p.Type == $"{classData.Namespace}.{classData.Name}");

                var associationBuilder = Builder.CreateAssociationBuilder(builderMetadataManager);
                if (bidirectionalProperty is null)
                {
                    // if the class contains a FK to the other class PK (based on convention)
                    var associationKeys = classData.Properties.Where(p => p.Name == $"{otherClass.Name}Id");

                    associationBuilder.AddUnidirectionalSource(sourceClassReference: $"{classData.Namespace}.{classData.Name}", null, false, associationKeys.Any());
                }
                else
                {
                    associationBuilder.AddBidirectionalSource(sourceClassReference: $"{classData.Namespace}.{classData.Name}",
                        bidirectionalFieldName: bidirectionalProperty.Name,
                        bidirectionalIsNullable: bidirectionalProperty.IsNullable,
                        bidirectionalIsCollection: bidirectionalProperty.IsCollection);
                }

                associationBuilder.AddTarget(targetClassReference: $"{otherClass.Namespace}.{otherClass.Name}",
                    targetFieldName: property.Name,
                    targetIsNullable: property.IsNullable,
                    targetIsCollection: property.IsCollection);

                var association = associationBuilder.Build();

                if (!standardLookup.HasExistingAssociation(association))
                {
                    associations.Add(association);
                    standardLookup.AddAssociation(association);
                }
            }
        }

        foreach (var (classData, classBuilder) in classDataAndBuilders)
        {
            classBuilder.AddConstructors(classData.Constructors,
                ctor => ctor.AddParameters(source => source.Parameters,
                    param => new(param.Name, param.Type!, param.IsNullable, param.IsCollection)));

            classBuilder.AddAttributes(classData.Properties.Where(p => !IsDomainType(p.Type!, standardLookup)),
                prop =>
                {
                    var attBuilder = new AttributeBuilder(prop.Name, prop.Type!, prop.IsNullable, prop.IsCollection);

                    // if the property is flagged with the [Key] atribute, is called "Id", or is called {ClassName}Id - then assume its a PK
                    if (prop.Attributes.Contains("Key") || prop.Name.Equals("Id", StringComparison.InvariantCultureIgnoreCase) ||
                        prop.Name.Equals($"{classData.Name}Id", StringComparison.InvariantCultureIgnoreCase))
                    {
                        attBuilder.AddStereotype("b99aac21-9ca4-467f-a3a6-046255a9eed6", "Primary Key", []);
                    }

                    // check if there are any associations where the source is this class, and target end is a table which matches the current column
                    var qualifyingAssociations = associations.Where(a =>
                    {
                        if(!standardLookup.TryGetElementById(a.TargetEnd.TypeReference.TypeId, out var targetElement))
                        {
                            return false;
                        }

                        // this use case caters for when the source end has the navigation property and the foreign key.
                        if(a.SourceEnd.TypeReference?.TypeName == classData.Name &&
                            targetElement.Name == prop.Name.Replace("Id", ""))
                        {
                            return true;
                        }

                        // this caters for when the target end has the foreign key and the navigation property is in the source end 
                        if (!standardLookup.TryGetElementById(a.SourceEnd?.TypeReference?.TypeId, out var sourceElement))
                        {
                            return false;
                        }

                        return targetElement.Name == classData.Name && sourceElement.Name == prop.Name.Replace("Id", "");
                    });

                    if (qualifyingAssociations.Any())
                    {
                        foreach (var association in qualifyingAssociations)
                        {
                            attBuilder.AddStereotype("793a5128-57a1-440b-a206-af5722b752a6", "Foreign Key",
                            [
                                new StereotypeProperty("42e4f9b5-f834-4e5f-86aa-d3a35c505076", Display: "Association", Value: association.TargetEnd.Id)
                            ]);
                        }
                    }

                    return attBuilder;
                });

            classBuilder.AddOperations(classData.Methods,
                m => m.Name,
                method => method.AddParameters(source => source.Parameters,
                    param => new (param.Name, param.Type!, param.IsNullable, param.IsCollection)));

            elements.Add(classBuilder.Build());
        }
    }
    
    private static void PostProcessValueObjects(
        List<(ClassData, ValueObjectBuilder)> valueObjectAndBuilders,
        List<ElementPersistable> elements)
{
    foreach (var (valueObjectData, valueObjectBuilder) in valueObjectAndBuilders)
    {
        valueObjectBuilder.AddAttributes(valueObjectData.Properties,
            prop => new(prop.Name, prop.Type!, prop.IsNullable, prop.IsCollection));

        elements.Add(valueObjectBuilder.Build());
    }
}

private static void PostProcessDataContracts(
    List<(ClassData, DomainDataContractBuilder)> dataContractAndBuilders,
    List<ElementPersistable> elements)
{
    foreach (var (domainDataContractData, domainDataContractBuilder) in dataContractAndBuilders)
    {
        domainDataContractBuilder.AddAttributes(domainDataContractData.Properties,
            prop => new(prop.Name, prop.Type!, prop.IsNullable, prop.IsCollection));

        elements.Add(domainDataContractBuilder.Build());
    }
}

private static void PostProcessDomainServices(List<(ClassData, DomainServiceElementBuilder)> domainServicesAndBuilders, List<ElementPersistable> elements)
{
    foreach (var (domainServiceData, domainServiceBuilder) in domainServicesAndBuilders)
    {
        domainServiceBuilder.AddOperations(domainServiceData.Methods,
            m => m.IsAsync ? $"{m.Name.RemoveSuffix("Async")}Async" : m.Name,
            method =>
            {
                method.AddGenericParameters(method.DataSource.GenericParameters);
                method.ReturnType(method.DataSource.ReturnType, method.DataSource.IsNullable, method.DataSource.ReturnsCollection);
                method.AddParameters(source => source.Parameters.Where(WithoutCancellationToken),
                    param => new(param.Name, param.Type!, param.IsNullable, param.IsCollection));
            });
        elements.Add(domainServiceBuilder.Build());
    }
}

private static void PostProcessRepositories(List<(ClassData, DomainRepositoryBuilder)> domainRepositoriesAndBuilders, List<ElementPersistable> elements)
{
    foreach (var (domainRepositoryData, domainRepositoryBuilder) in domainRepositoriesAndBuilders)
    {
        domainRepositoryBuilder.AddOperations(domainRepositoryData.Methods,
            m => m.IsAsync ? $"{m.Name.RemoveSuffix("Async")}Async" : m.Name,
            method =>
            {
                method.AddGenericParameters(method.DataSource.GenericParameters);
                method.ReturnType(method.DataSource.ReturnType, method.DataSource.IsNullable, method.DataSource.ReturnsCollection);
                method.AddParameters(source => source.Parameters.Where(WithoutCancellationToken),
                    param => new(param.Name, param.Type!, param.IsNullable, param.IsCollection));
            });

        elements.Add(domainRepositoryBuilder.Build());
    }
}

private static void PostProcessDtos(List<(ClassData, DtoElementBuilder)> serviceDtosAndBuilders, MetadataLookup standardLookup, List<ElementPersistable> elements)
{
    foreach (var (serviceDtoData, dtoBuilder) in serviceDtosAndBuilders)
    {
        dtoBuilder.AddFields(serviceDtoData.Properties.Where(p => !IsDomainType(p.Type!, standardLookup)),
            prop => new(prop.Name, prop.Type!, prop.IsNullable, prop.IsCollection));

        elements.Add(dtoBuilder.Build());
    }
}

private static void PostProcessEventMessages(List<(ClassData, EventMessageElementBuilder)> eventMessagesAndBuilders, MetadataLookup standardLookup, List<ElementPersistable> elements)
{
    // add all properties to the messages
    foreach (var (eventMessageData, eventMessageBuilder) in eventMessagesAndBuilders)
    {
        eventMessageBuilder.AddProperties(eventMessageData.Properties,
            prop => new(prop.Name, prop.Type!, prop.IsNullable, prop.IsCollection));

        elements.Add(eventMessageBuilder.Build());
    }
}

private static void PostProcessEventDtos(List<(ClassData, EventDtoElementBuilder)> eventMessagesAndBuilders, MetadataLookup standardLookup, List<ElementPersistable> elements)
{
    // add all properties to the messages
    foreach (var (eventMessageData, eventMessageBuilder) in eventMessagesAndBuilders)
    {
        eventMessageBuilder.AddFields(eventMessageData.Properties,
            prop => new(prop.Name, prop.Type!, prop.IsNullable, prop.IsCollection));

        elements.Add(eventMessageBuilder.Build());
    }
}

private static void RegisterCommonEnum(string targetFolderPath, string? designerTargetFolderId, BuilderMetadataManager builderMetadataManager,
    List<ElementPersistable> elements, MetadataLookup standardLookup, EnumData enumData)
{
    var enumBuilder = Builder.CreateEnumBuilder(
        externalReference: $"{enumData.Namespace}.{enumData.Name}",
        name: enumData.Name,
        builderMetadataManager: builderMetadataManager);

    foreach (var literal in enumData.Literals)
    {
        enumBuilder.AddLiteral(literal.Name, literal.Value);
    }

    elements.Add(enumBuilder.Build());

    var folders = GetFolderElements(
        folderPath: GetRelativeLocation(enumData.FilePath, targetFolderPath),
        lookup: standardLookup,
        targetFolderId: designerTargetFolderId);
    enumBuilder.SetParentId(folders.LastOrDefault()?.Id);
    elements.AddRange(standardLookup.AddElementsIfMissing(folders));
}

private static bool IsDomainType(string type, MetadataLookup standardLookup)
{
    return standardLookup.TryGetElementByReference(type, ClassModel.SpecializationTypeId, out _);
}

private static readonly char[] Separators = { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };

private static IReadOnlyList<ElementPersistable> GetFolderElements(string folderPath, MetadataLookup lookup, string? targetFolderId)
{
    if (string.IsNullOrWhiteSpace(folderPath) || folderPath == ".")
    {
        return ArraySegment<ElementPersistable>.Empty;
    }

    var folders = new List<ElementPersistable>();
    var parentFolderId = targetFolderId;
    var parts = folderPath.Split(Separators, StringSplitOptions.RemoveEmptyEntries);
    for (var partIndex = 0; partIndex < parts.Length; partIndex++)
    {
        var curPart = parts[partIndex];
        if (curPart == ".")
        {
            continue;
        }

        var pathToHere = string.Join("-", parts.Take(partIndex + 1));
        var reference = $"Folder-{pathToHere}";

        if (!lookup.TryGetElementByReference(reference, FolderModel.SpecializationTypeId, out var folderElement))
        {
            folderElement = ElementPersistable.Create(
                specializationType: FolderModel.SpecializationType,
                specializationTypeId: FolderModel.SpecializationTypeId,
                name: curPart,
                parentId: parentFolderId,
                externalReference: reference);
        }

        parentFolderId = folderElement.Id;

        folders.Add(folderElement);
    }

    return folders;
}

private static string GetRelativeLocation(string? curFilePath, string targetFolder)
{
    var curClassDir = Path.GetDirectoryName(curFilePath);
    if (string.IsNullOrWhiteSpace(curClassDir))
    {
        curClassDir = ".";
    }

    var newPath = Path.GetRelativePath(targetFolder, curClassDir);
    newPath = newPath.TrimStart('.', '\\', '/');
    return newPath;
}
}