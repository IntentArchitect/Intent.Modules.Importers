using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using SharpYaml.Tokens;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Xml.Linq;

namespace Intent.MetadataSynchronizer.OpenApi.CLI.ServiceCreation
{
    public abstract class ServiceCreationStrategyBase : IServiceCreationStrategy
    {
        private protected readonly MetadataLookup MetadataLookup;
        private protected ImportConfig Config;
        private protected IOpenApiPresistableFactory OpenApiHelper;
        private protected readonly Dictionary<string, ElementPersistable> _addedServiceTypes;
        private protected readonly Dictionary<string, ElementPersistable> _folders;
        private protected DomainImplementation _domain;

        public ServiceCreationStrategyBase(MetadataLookup metadataLookup, ImportConfig config, IOpenApiPresistableFactory openApiHelper)
        {
            _addedServiceTypes = new Dictionary<string, ElementPersistable>();
            _domain = new DomainImplementation(metadataLookup, config, _addedServiceTypes);
            MetadataLookup = metadataLookup;
            Config = config;
            OpenApiHelper = openApiHelper;
            _folders = new Dictionary<string, ElementPersistable>();
        }

        protected ElementPersistable CreateOrGetDto(ResolvedType? type, string specializationType, string specializationTypeId, ElementPersistable folder)
        {
            var typeName = type!.TypeName.ToPascalCase();
            var externalReference = $"{folder.Name}.{typeName}";
            return CreateOrGetDto(type, specializationType, specializationTypeId, typeName, folder, externalReference);
        }

        protected ElementPersistable CreateOrGetDto(
            ResolvedType? type,
            string specializationType,
            string specializationTypeId,
            string name, 
            ElementPersistable folder,
            string externalReference)
        {
            var key = externalReference;

            if (_addedServiceTypes.TryGetValue(key, out var result))
            {
                return result;
            }

            if (name == "JsonResponse" && type != null)
            {
                var start = type.OpenApiType.Reference.Id.IndexOf("JsonResponse`1[[", StringComparison.Ordinal) + "JsonResponse`1[[".Length;
                var end = type.OpenApiType.Reference.Id.IndexOf(",", start, StringComparison.Ordinal);
                var fullTypeName = type.OpenApiType.Reference.Id.Substring(start, end - start);
                var wrappedTypeName = fullTypeName[(fullTypeName.LastIndexOf(".", StringComparison.Ordinal) + 1)..];
                result = OpenApiHelper.GetIntentType(wrappedTypeName.ToLower());

                return result;
            }

            result = ElementPersistable.Create(
                specializationType: specializationType,
                specializationTypeId: specializationTypeId,
                name: SanitizeTypeName(name),
                parentId: folder.Id,
                externalReference: externalReference);
            _addedServiceTypes.Add(key, result);

            if (type == null)
            {
                return result;
            }

            if (type.OpenApiType.AllOf != null && type.OpenApiType.AllOf.Any())
            {
                foreach (var child in type.OpenApiType.AllOf)
                {
                    AddProperties(result, child, folder);
                }
            }
            else
            {
                AddProperties(result, type.OpenApiType, folder);
            }

            return result;
        }

        private void AddProperties(ElementPersistable result, OpenApiSchema openApiType, ElementPersistable folder)
        {
            /* Required is an optional concept.
             * If required is present it means that all fields not listed here need to be nullable
             */
            bool impliedNullables = openApiType.Required.Any();

            foreach (var property in openApiType.Properties)
            {
                var propertyExternalReference = $"{result.ExternalReference}.{property.Key.ToPascalCase()}";

                if (!_addedServiceTypes.TryGetValue(propertyExternalReference, out var propertyElement))
                {
                    propertyElement = ElementPersistable.Create(
                        specializationType: DTOFieldModel.SpecializationType,
                        specializationTypeId: DTOFieldModel.SpecializationTypeId,
                        name: property.Key.ToPascalCase(),
                        parentId: result.Id,
                        externalReference: propertyExternalReference);

                    _addedServiceTypes.Add(propertyElement.ExternalReference, propertyElement);
                }

                propertyElement.TypeReference = GetIntentType(OpenApiHelper.Resolve(property.Value, typeNameContext: property.Key, impliedNullables: impliedNullables, requiredProperties: openApiType.Required), folder);
            }
        }

        private ElementPersistable CreateOrGetEnum(ResolvedType schemaType, ElementPersistable folder)
        {
            var externalReference = $"{folder.Name}.{schemaType.TypeName.ToPascalCase()}";

            if (_addedServiceTypes.TryGetValue(externalReference, out var result))
            {
                return result;
            }

            var literals = new List<EnumLiteralsInfo>();

            if (schemaType.OpenApiType.AllOf != null && schemaType.OpenApiType.AllOf.Any())
            {
                foreach (var child in schemaType.OpenApiType.AllOf)
                {
                    AddLiterals(child, literals);
                }
            }
            else
            {
                AddLiterals(schemaType.OpenApiType, literals);
            }

            result = ElementPersistable.Create(
                specializationType: EnumModel.SpecializationType,
                specializationTypeId: EnumModel.SpecializationTypeId,
                name: SanitizeTypeName(schemaType.TypeName),
                parentId: folder.Id,
                externalReference: externalReference);
            _addedServiceTypes.Add(externalReference, result);

            foreach (var literal in literals)
            {
                var propertyElement = ElementPersistable.Create(
                    specializationType: EnumLiteralModel.SpecializationType,
                    specializationTypeId: EnumLiteralModel.SpecializationTypeId,
                    name: literal.Name.ToPascalCase(),
                    parentId: result.Id,
                    externalReference: $"{externalReference}.{literal.Name.ToPascalCase()}");

                if (literal.Value != null)
                {
                    propertyElement.Value = literal.Value.Value.ToString();
                }

                _addedServiceTypes.Add(propertyElement.ExternalReference, propertyElement);
            }

            return result;
        }

        private void AddLiterals(OpenApiSchema openApiType, List<EnumLiteralsInfo> literals )
        {
            for (int i = 0; i < openApiType.Enum.Count; i++)
            {
                var literal = openApiType.Enum[i];
                switch (literal)
                {
                    case OpenApiString sl:
                        literals.Add(new EnumLiteralsInfo(sl.Value));
                        break;
                    case OpenApiInteger il:
                        if (openApiType.Extensions.TryGetValue("x-enumNames", out var enumNamesExtension))
                        {
                            var enumNames = (enumNamesExtension as Microsoft.OpenApi.Any.OpenApiArray)?
                                .OfType<OpenApiString>()
                                .Select(e => e.Value)
                                .ToList();
                            if (enumNames != null && enumNames.Count > i)
                            {
                                literals.Add(new EnumLiteralsInfo(enumNames[i] ?? "", il.Value));
                            }
                            else
                            {
                                literals.Add(new EnumLiteralsInfo(il.Value));
                            }
                        }
                        else
                        {
                            literals.Add(new EnumLiteralsInfo(il.Value));
                        }
                        break;
                    case OpenApiNull:
                        break;
                    default:
                        throw new NotSupportedException($"Only support Enum types of (string,integer). {literal.GetType().Name} is unsupported.");
                }
            }
        }

        protected ElementPersistable GetOrCreateFolder(string conceptName)
        {
            var key = $"Folder:{conceptName}";
            if (_folders.TryGetValue(key, out var folder))
            {
                return folder;
            }

            folder = ElementPersistable.Create(
                specializationType: FolderModel.SpecializationType,
                specializationTypeId: FolderModel.SpecializationTypeId,
                name: conceptName.Pluralize().ToPascalCase(),
                parentId: null,
                externalReference: conceptName.Pluralize().ToPascalCase());

            _folders.Add(key, folder);
            _addedServiceTypes.Add(key, folder);

            return folder;
        }

        protected void AddParameters(AbstractServiceOperationModel serviceDefinition,
            string specializationType,
            string specializationTypeId,
            ElementPersistable parentElement,
            string externalReferencePrefix,
            Func<string, string> format)
        {
            if (serviceDefinition.Parameters == null || !serviceDefinition.Parameters.Any())
            {
                return;
            }

            foreach (var parameter in serviceDefinition.Parameters)
            {
                var parameterName = format(parameter.ParameterInfo.Name);
                var externalReference = $"{externalReferencePrefix}.{parentElement.Name}.{parameterName}";

                //Can be added as a Swagger parameter and from the Swagger body // don't add a duplicate
                if (_addedServiceTypes.TryGetValue(externalReference, out _))
                    continue;

                var propertyElement = ElementPersistable.Create(
                    specializationType: specializationType,
                    specializationTypeId: specializationTypeId,
                    name: parameterName,
                    parentId: parentElement.Id,
                    externalReference: externalReference);

                _addedServiceTypes.Add(propertyElement.ExternalReference, propertyElement);

                var schema = OpenApiHelper.Resolve(parameter.ParameterInfo.Schema, parameter.ParameterInfo.Name);

                var referencedType = schema.SwaggerType != "object"
                    ? OpenApiHelper.GetIntentType(schema.SwaggerType)
                    : CreateOrGetDto(
                        schema,
                        DTOModel.SpecializationType,
                        DTOModel.SpecializationTypeId,
                        parentElement);

                propertyElement.TypeReference = TypeReferencePersistable.Create(
                    typeId: referencedType.Id,
                    isNavigable: true,
                    isNullable: schema.IsNullable,
                    isCollection: schema.IsCollection,
                    isRequired: default,
                    comment: default,
                    genericTypeId: default,
                    typePackageName: referencedType.PackageName,
                    typePackageId: referencedType.PackageId,
                    stereotypes: [],
                    genericTypeParameters: []);

                switch (parameter.ParameterInfo.In)
                {
                    case ParameterLocation.Header:
                        propertyElement.Stereotypes.Add(CreateParameterSettingsStereotype("From Header", parameter.ParameterInfo.Name));
                        break;
                    case ParameterLocation.Query:
                        propertyElement.Stereotypes.Add(CreateParameterSettingsStereotype("From Query"));
                        break;
                }
            }
        }

        private static string SanitizeTypeName(string originalName)
        {
            if (string.IsNullOrEmpty(originalName))
            {
                return originalName;
            }
            var altered = originalName;
            if (char.IsDigit(altered[0]))
            {
                altered = "_" + altered;
            }
            return altered.ToPascalCase();
        }

        protected static StereotypePersistable CreateParameterSettingsStereotype(string source, string? headerName = null)
        {
            return new StereotypePersistable
            {
                DefinitionId = "d01df110-1208-4af8-a913-92a49d219552",
                Name = "Parameter Settings",
                Comment = null,
                AddedByDefault = false,
                DefinitionPackageName = "Intent.Metadata.WebApi",
                DefinitionPackageId = "0011387a-b122-45d7-9cdb-8e21b315ab9f",
                Properties =
                [
                    new StereotypePropertyPersistable { DefinitionId = "d2630e0f-f930-404f-b453-1e8052a712f5", Name = "Source", Value = source },
                    new StereotypePropertyPersistable { DefinitionId = "7a331e9b-f13c-4b33-9013-bd59b4a4999c", Name = "Header Name", Value = headerName }
                ]
            };
        }

        protected static StereotypePersistable CreateHttpSettingsStereotype(string verb, string route, string mediaType, string? restSuccessCode)
        {
            var result = new StereotypePersistable
            {
                DefinitionId = "b4581ed2-42ec-4ae2-83dd-dcdd5f0837b6",
                Name = "Http Settings",
                Comment = null,
                AddedByDefault = false,
                DefinitionPackageName = "Intent.Metadata.WebApi",
                DefinitionPackageId = "0011387a-b122-45d7-9cdb-8e21b315ab9f",
                Properties =
                [
                    new StereotypePropertyPersistable { DefinitionId = "801c3e61-4431-4d81-93fa-7e57d33cb3fa", Name = "Verb", Value = verb },
                    new StereotypePropertyPersistable { DefinitionId = "5dd3e07d-76eb-45d4-9956-4325fb068acc", Name = "Route", Value = route },
                    new StereotypePropertyPersistable
                    {
                        DefinitionId = "4490e212-1e99-43ce-b3dd-048ed2a6bae8", 
                        Name = "Return Type Mediatype",
                        Value = mediaType
                    }

                ]
            };
            if (restSuccessCode != null && restSuccessCode != "200")
            {
                result.Properties.Add(
                    new StereotypePropertyPersistable
                    {
                        DefinitionId = "e3870725-34b3-4684-85f2-ec4a667207fb",
                        Name = "Success Response Code",
                        Value = GetResponseCodeValue(restSuccessCode)
                    }
                    );
            }
            return result;
        }

        private static string GetResponseCodeValue(string restSuccessCode)
        {
            switch (restSuccessCode)
            {
                case "201":
                    return "201 (Created)";
                case "202":
                    return "202 (Accepted)";
                case "203":
                    return "203 (Non-Authoritative Information)";
                case "204":
                    return "204 (No Content)";
                case "205":
                    return "205 (Reset Content)";
                case "206":
                    return "206 (Partial Content)";
                case "207":
                    return "207 (Multi-Status)";
                case "208":
                    return "208 (Already Reported)";
                case "226":
                    return "226 (IM Used)";
                default:
                    return null;
            }
        }

        protected static StereotypePersistable CreateAzureFunctionStereotype()
        {
            return new StereotypePersistable
            {
                DefinitionId = "7c1128f6-fdef-4bf9-8f15-acb54b5bfa89",
                Name = "Azure Function",
                Comment = null,
                AddedByDefault = false,
                DefinitionPackageName = "Intent.AzureFunctions",
                DefinitionPackageId = "022dbddc-276c-406b-81e6-5034ad64db72",
                Properties =
                [
                    new StereotypePropertyPersistable { DefinitionId = "a6411e1f-8199-4b18-b1a1-fd2aa73b1da6", Name = "Trigger", Value = "Http Trigger" },
                    new StereotypePropertyPersistable
                    {
                        DefinitionId = "bb4eeea4-a572-416e-8af9-8b56743879ea", Name = "Authorization Level", Value = "Function"
                    },
                    new StereotypePropertyPersistable { DefinitionId = "ba0f1189-f620-4874-8b6a-af71bf30f3ba", Name = "Queue Name" },
                    new StereotypePropertyPersistable { DefinitionId = "8267af60-0b1f-4394-8148-89b2ef9c16bf", Name = "Schedule Expression" },
                    new StereotypePropertyPersistable { DefinitionId = "ef3822f1-53c5-4942-a1a0-2433f1a8a4dc", Name = "EventHub Name" },
                    new StereotypePropertyPersistable { DefinitionId = "c0726eed-bffd-420e-9960-5e1f1ec2b44a", Name = "Connection" },
                    new StereotypePropertyPersistable
                    {
                        DefinitionId = "a83b939c-86c5-498e-8e64-006a9e4cdc83", Name = "Return Type Mediatype", Value = "Default"
                    },
                    new StereotypePropertyPersistable { DefinitionId = "02abfe51-f96a-4ce0-be98-e007ea84f374", Name = "Route" },
                    new StereotypePropertyPersistable { DefinitionId = "2e05aa2c-ccb6-4a55-861e-194b5dbb4aba", Name = "Method" }
                ]
            };
        }

        private ElementPersistable GetDictionaryType()
        {
            if (MetadataLookup.TryGetTypeDefinitionByName("Dictionary", 2, out var dictionaryType))
            {
                return dictionaryType;
            }

            if (MetadataLookup.TryGetTypeDefinitionByName("Map", 2, out dictionaryType))
            {
                return dictionaryType;
            }

            if (MetadataLookup.TryGetElementById("ac2b65c3-6a8f-454a-b520-b583350c43ef", out dictionaryType))
            {
                return dictionaryType;
            }

            throw new Exception("Unable to resolve a (Dictionary/Map) type consider installing 'Intent.Common.CSharp' or 'Intent.Common.Java' module ");
        }

        protected TypeReferencePersistable GetIntentType(ResolvedType schemaType, ElementPersistable folder)
        {
            ElementPersistable referencedType;

            switch (schemaType.SwaggerType)
            {
                case "object" when schemaType.OpenApiType.AdditionalProperties != null:
                    {
                        //Generic Dictionary<string, TValue>
                        var dictionaryType = GetDictionaryType();

                        return TypeReferencePersistable.Create(
                            typeId: dictionaryType.Id,
                            isNavigable: true,
                            isNullable: schemaType.IsNullable,
                            isCollection: schemaType.IsCollection,
                            isRequired: default,
                            comment: default,
                            genericTypeId: default,
                            typePackageName: dictionaryType.PackageName,
                            typePackageId: dictionaryType.PackageId,
                            stereotypes: [],
                            genericTypeParameters:
                            [
                                Create(OpenApiHelper.GetIntentType("string"), dictionaryType.GenericTypes[0].Id),
                            Create(
                                OpenApiHelper.GetIntentType(OpenApiHelper
                                    .Resolve(schemaType.OpenApiType.AdditionalProperties).SwaggerType),
                                dictionaryType.GenericTypes[1].Id)
                            ]);
                    }
                case "object":
                    referencedType = CreateOrGetDto(
                        schemaType,
                        DTOModel.SpecializationType, DTOModel.SpecializationTypeId,
                        folder);
                    break;
                case "enum":
                    referencedType = CreateOrGetEnum(schemaType, folder);
                    break;
                default:
                    referencedType = OpenApiHelper.GetIntentType(schemaType.SwaggerType);
                    break;
            }

            return TypeReferencePersistable.Create(
                typeId: referencedType.Id,
                isNavigable: true,
                isNullable: schemaType.IsNullable,
                isCollection: schemaType.IsCollection,
                isRequired: default,
                comment: default,
                genericTypeId: default,
                typePackageName: referencedType.PackageName,
                typePackageId: referencedType.PackageId,
                stereotypes: [],
                genericTypeParameters: []);
        }

        private static TypeReferencePersistable Create(ElementPersistable elementPersistable, string genericTypeId)
        {
            return TypeReferencePersistable.Create(
                typeId: elementPersistable.Id,
                isNavigable: true,
                isNullable: false,
                isCollection: false,
                isRequired: default,
                comment: default,
                genericTypeId: genericTypeId,
                typePackageName: elementPersistable.PackageName,
                typePackageId: elementPersistable.PackageId,
                stereotypes: [],
                genericTypeParameters: []);
        }

        public Persistables GetDomainPersistables()
        {
            return _domain.GetDomainPersistables();
        }

        public Persistables CreateServices(IList<AbstractServiceOperationModel> serviceDefinitions)
        {
            var result = CreateServicesInternal(serviceDefinitions);
            if (Config.ReverseEngineerImplementation)
            {

                var toMove = result.Elements.Where(e => e.SpecializationTypeId == EnumModel.SpecializationTypeId || e.SpecializationTypeId == EnumLiteralModel.SpecializationTypeId);
                result = new Persistables(result.Elements.Except(toMove).ToList(), result.Associations);
                _domain.AddEnums(toMove);
            }
            return result;
        }

        protected abstract Persistables CreateServicesInternal(IList<AbstractServiceOperationModel> serviceDefinitions);

        private class EnumLiteralsInfo
        {
            public string Name { get; }
            public int? Value { get; }

            public EnumLiteralsInfo(string name, int? value = null)
            {
                Name = name;
                Value = value;
            }

            public EnumLiteralsInfo(int value) : this ($"Option{value}", value)
            {
            }

            public EnumLiteralsInfo(string name) : this (name, null)
            {
            }
        }
    }
}