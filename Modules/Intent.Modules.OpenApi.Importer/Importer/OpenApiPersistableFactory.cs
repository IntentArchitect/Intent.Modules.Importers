using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.MetadataSynchronizer;
using Intent.Modules.OpenApi.Importer.Importer.ServiceCreation;
using Intent.Modules.Common.Templates;
using Intent.Utils;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using Microsoft.OpenApi.Validations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Text;

namespace Intent.Modules.OpenApi.Importer.Importer
{
    public class OpenApiPersistableFactory : IOpenApiPresistableFactory
    {
        private readonly Dictionary<string, string> _openApiTypeMapping = new();
        private readonly Dictionary<string, ElementPersistable> _intentTypeLookup = new();
        private Dictionary<string, OpenApiSchema> _swaggerComponentsLookup;
        private ElementPersistable _fallbackType;
        private MetadataLookup _metadataLookup;
        private ImportConfig _config;
        private IServiceCreationStrategy _metadataFactory;
        private OpenApiDocument _document;

        public OpenApiPersistableFactory()
        {
            LoadOpenApiTypeMapping();
            _fallbackType = null!;
            _config = null!;
            _metadataLookup = null!;
            _metadataFactory = null!;
            _swaggerComponentsLookup = null!;
        }

        private void LoadOpenApiTypeMapping()
        {
            _openApiTypeMapping.Add("string", "string");
            _openApiTypeMapping.Add("byte", "byte");
            _openApiTypeMapping.Add("password", "string");
            _openApiTypeMapping.Add("email", "string");
            _openApiTypeMapping.Add("hostname", "string");
            _openApiTypeMapping.Add("ipv4", "string");
            _openApiTypeMapping.Add("ipv6", "string");
            _openApiTypeMapping.Add("object", "object");
            _openApiTypeMapping.Add("int32", "int");
            _openApiTypeMapping.Add("integer", "int");
            _openApiTypeMapping.Add("int64", "int");
            _openApiTypeMapping.Add("float", "float");
            _openApiTypeMapping.Add("number", "decimal");
            _openApiTypeMapping.Add("double", "double");
            _openApiTypeMapping.Add("boolean", "bool");
            _openApiTypeMapping.Add("date", "date");
            _openApiTypeMapping.Add("date-time", "datetimeoffset");
            _openApiTypeMapping.Add("uuid", "guid");
        }

        public ElementPersistable GetIntentType(string openApiType)
        {
            if (!_intentTypeLookup.TryGetValue(openApiType, out var intentType))
            {
                if (_openApiTypeMapping.TryGetValue(openApiType, out var mappedType))
                {
                    if (!_metadataLookup.TryGetTypeDefinitionByName(mappedType, 0, out intentType))
                        throw new Exception($"Unknown Intent Type : '{mappedType}'");
                }
                else
                {
                    if (!_metadataLookup.TryGetTypeDefinitionByName(openApiType, 0, out intentType))
                    {
                        intentType = _fallbackType;
                    }
                }

                _intentTypeLookup.Add(openApiType, intentType);
            }

            return intentType;
        }

        public Persistables GetPersistables(
            ImportConfig config,
            IReadOnlyCollection<PackageModelPersistable> packages)
        {
            using var stream = GetRelevantStream(config.OpenApiSpecificationFile);

            return GetPersistables(
                stream: stream,
                config: config,
                packages: packages);
        }

        public Persistables GetPersistables(
            Stream stream,
            ImportConfig config,
            IReadOnlyCollection<PackageModelPersistable> packages)
        {
            var document = LoadDocument(stream);
            _document = document;

            Configure(packages, config, document);

            var services = ParseServices(config, document);
            return _metadataFactory.CreateServices(services);
        }

        public Persistables GetDomainPersistables()
        {
            return _metadataFactory.GetDomainPersistables();
        }


        private OperationDetails GetServiceDetails(string restType, string url, OpenApiOperation operation)
        {
            string serviceRoute = "";
            string operationRoute = "";
            string serviceName = "";
            var conceptName = new List<string>();
            var split = url.Split("/");
            for (int i = 0; i < split.Length; i++)
            {
                if (serviceName == "")
                {
                    serviceRoute = string.Join("/", split.Take(i + 1));
                    operationRoute = string.Join("/", split.Skip(i + 1));
                    if (split[i].ToLower() != "api")
                    {
                        serviceName = split[i].Singularize().ToPascalCase();
                    }
                }
                else if (!split[i].Contains("{"))
                {
                    conceptName.Add(split[i].ToPascalCase());
                }
            }

            return new OperationDetails(serviceRoute: serviceRoute, operationRoute: operationRoute, serviceName: serviceName, conceptName: conceptName, restType: restType,
                operation: operation);
        }

        private IList<AbstractServiceOperationModel> ParseServices(ImportConfig config, OpenApiDocument document)
        {
            var result = new List<AbstractServiceOperationModel>();

            _swaggerComponentsLookup = new Dictionary<string, OpenApiSchema>();

            foreach (var schemaKvp in document.Components.Schemas)
            {
                _swaggerComponentsLookup.Add(schemaKvp.Key, schemaKvp.Value);
            }

            var operations = new List<OperationDetails>();

            foreach (var path in document.Paths)
            {
                string url = path.Key;
                foreach (var operationKvp in path.Value.Operations)
                {
                    string restType = operationKvp.Key.ToString().ToLower();
                    var operation = operationKvp.Value;

                    var operationDetails = GetServiceDetails(restType, url, operation);
                    operations.Add(operationDetails);
                    ResolvedType? response = null;
                    var successResponses = operation.Responses.FirstOrDefault(r => r.Key.StartsWith("2")).Value;
                    if (successResponses != null && successResponses.Content != null && successResponses.Content.Count > 0)
                    {
                        if (!successResponses.Content.TryGetValue("application/json", out var content))
                        {
                            content = successResponses.Content.FirstOrDefault().Value;
                        }

                        if (!IsNull(content.Schema))
                        {
                            response = Resolve(content.Schema, operationDetails.OperationName);
                            if (response.IsCollection)
                            {
                                operationDetails.PluralizeOperationName();
                            }
                        }
                    }
                }
            }

            var duplicates = operations
                .GroupBy(o => new { o.ServiceName, o.OperationName })
                .Where(g => g.Count() > 1)
                .SelectMany(g => g.Select((op, index) => new { Operation = op, Index = index }))
                .ToList();

            //Rename duplicate operations
            foreach (var item in duplicates)
            {
                if (item.Index > 0)
                {
                    item.Operation.OperationName += item.Index;
                }
            }

            foreach (var operationDetails in operations)
            {
                var operation = operationDetails.Operation;
                ResolvedType? response = null;
                var successResponses = operation.Responses.FirstOrDefault(r => r.Key.StartsWith("2")).Value;
                if (successResponses != null && successResponses.Content != null && successResponses.Content.Count > 0)
                {
                    if (!successResponses.Content.TryGetValue("application/json", out var content))
                    {
                        content = successResponses.Content.FirstOrDefault().Value;
                    }

                    if (!IsNull(content.Schema))
                    {
                        response = Resolve(content.Schema, operationDetails.OperationName);
                        if (response.IsCollection)
                        {
                            operationDetails.PluralizeOperationName();
                            response = Resolve(content.Schema, operationDetails.OperationName);
                        }
                    }
                }

                string? restSuccessCode = null;
                if (operation.Responses.Count(r => r.Key.StartsWith("2")) == 1)
                {
                    restSuccessCode = operation.Responses.First(r => r.Key.StartsWith("2")).Key.Trim();
                }


                ResolvedType? bodyType = null;
                if (operation.RequestBody != null && operation.RequestBody.Content.Count > 0)
                {
                    if (!operation.RequestBody.Content.TryGetValue("application/json", out var content))
                    {
                        content = operation.RequestBody.Content.FirstOrDefault().Value;
                    }

                    if (!IsNull(content.Schema))
                        bodyType = Resolve(content.Schema, operationDetails.OperationName);
                }

                var parameters = new List<Parameter>();
                foreach (var x in operation.Parameters)
                {
                    parameters.Add(new Parameter(x, Resolve(x.Schema, x.Name)));
                }

                /*
                //This is technically how it should work but the NuGet package doesn't support this scenario, the operation "security" is always populated with an empty collection
                bool globallySecured = document.SecurityRequirements != null && document.SecurityRequirements.Any();
                bool locallySecured = operation.Security != null && operation.Security.Any();
                bool locallyUnsecured = operation.Security != null && !operation.Security.Any();
                var secured = locallySecured || (globallySecured && !locallyUnsecured);
                */
                var secured = operation.Security != null && operation.Security.Any();
                result.Add(new AbstractServiceOperationModel(operationDetails.RestType, operationDetails.ServiceRoute, operationDetails.ServiceName,
                    operationDetails.OperationRoute, operationDetails.OperationName, operationDetails.ConceptName.LastOrDefault() ?? operationDetails.ServiceName, bodyType,
                    response, parameters, secured, restSuccessCode));
            }

            return result;
        }

        public bool IsNull(OpenApiSchema schema)
        {
            return schema == null || (schema.Type == null && schema.Reference == null && schema.AllOf == null);
        }

        public ResolvedType Resolve(OpenApiSchema schema, string typeNameContext = null, bool impliedNullables = false, ISet<string> requiredProperties = null)
        {
            bool isCollection = schema.Type == "array";
            if (isCollection)
            {
                schema = schema.Items;
            }

            OpenApiSchema response = schema;
            if (response.Reference != null)
            {
                response = _swaggerComponentsLookup[response.Reference.Id];
            }

            bool nullable = !isCollection && (response.Nullable || response.Enum.Any(l => l is OpenApiNull));

            if (impliedNullables && !requiredProperties.Contains(typeNameContext))
            {
                nullable = true;
            }

            return new ResolvedType(response, isCollection, nullable, typeNameContext);
        }

        public void PersistAdditionalMetadata(PackageModelPersistable package)
        {
            if (_config.SettingPersistence != SettingPersistence.None)
            {
                package.AddMetadata("open-api-import:open-api-file", _config.OpenApiSpecificationFile);
                package.AddMetadata("open-api-import:add-postfixes", _config.AddPostFixes.ToString().ToLower());
                package.AddMetadata("open-api-import:allow-removal", _config.AllowRemoval.ToString().ToLower());
                package.AddMetadata("open-api-import:service-type", _config.ServiceType.ToString());
                package.AddMetadata("open-api-import:setting-persistence", _config.SettingPersistence.ToString());
            }
            else
            {
                package.RemoveMetadata("open-api-import:open-api-file");
                package.RemoveMetadata("open-api-import:add-postfixes");
                package.RemoveMetadata("open-api-import:allow-removal");
                package.RemoveMetadata("open-api-import:service-type");
                package.RemoveMetadata("open-api-import:setting-persistence");
            }

            if (!package.Stereotypes.Any(s => s.DefinitionId == "c06e9978-c271-49fc-b5c9-09833b6b8992"))
            {
                package.Stereotypes.Add(new StereotypePersistable
                {
                    DefinitionId = "c06e9978-c271-49fc-b5c9-09833b6b8992",
                    Name = "Endpoint Settings",
                    DefinitionPackageName = "Intent.Metadata.WebApi",
                    DefinitionPackageId = "0011387a-b122-45d7-9cdb-8e21b315ab9f",
                    Properties =
                        [
                            new StereotypePropertyPersistable
                            {
                                Name = "Service URL",
                                DefinitionId = "2164bf84-1db8-42d0-94a6-255d2908b9b5",
                                Value = GetOpenApiDocumentUrl()
                            }
                        ]
                });
            }
        }

        private string GetOpenApiDocumentUrl()
        {
            var httpsServer = _document.Servers.FirstOrDefault(s => s.Url.StartsWith("https"));
            if (httpsServer is not null && !string.IsNullOrWhiteSpace(httpsServer.Url))
            {
                return httpsServer.Url;
            }

            var httpServer = _document.Servers.FirstOrDefault(s => s.Url.StartsWith("http"));
            if (httpServer is not null && !string.IsNullOrWhiteSpace(httpServer.Url))
            {
                return httpServer.Url;
            }

            var uri = new Uri(_config.OpenApiSpecificationFile);
            if(_document.Servers.Any(s => !string.IsNullOrWhiteSpace(s.Url)) && 
                uri.Scheme == "http" || uri.Scheme == "https")
            {
                var serviceUrl = new Uri(uri, _document.Servers.First(s => !string.IsNullOrWhiteSpace(s.Url)).Url);

                return serviceUrl.ToString();
            }

            return string.Empty;
        }

        private static OpenApiDocument LoadDocument(Stream stream)
        {
            var settings = new OpenApiReaderSettings
            {
                RuleSet = ValidationRuleSet.GetDefaultRuleSet()
            };

            settings.RuleSet.Remove("KeyMustBeRegularExpression");
            
            var openApiDocument = new OpenApiStreamReader(settings).Read(stream, out var diagnostic);
            if (diagnostic is null || diagnostic.Errors.Count <= 0)
            {
                return openApiDocument;
            }

            var errorMessage = new StringBuilder();
            errorMessage.AppendLine("OpenAPI Parsing errors:");
            foreach (var error in diagnostic.Errors)
            {
                errorMessage.AppendLine($" - {error}");
            }

            throw new Exception(errorMessage.ToString());
        }

        private static Stream GetRelevantStream(string sourceFile)
        {
            var uri = new Uri(sourceFile);
            switch (uri.Scheme)
            {
                case "http":
                case "https":
                    var client = new HttpClient();
                    var httpContent = client.GetAsync(sourceFile).Result;
                    var memoryStream = new MemoryStream();
                    httpContent.Content.CopyToAsync(memoryStream).Wait();
                    memoryStream.Position = 0;
                    return memoryStream;
                case "file":
                    if (!File.Exists(sourceFile))
                    {
                        throw new Exception($"Unable to find file : {sourceFile}");
                    }

                    return new FileStream(sourceFile, FileMode.Open);
                default:
                    throw new NotSupportedException($"Unsupported for '{sourceFile}'");
            }
        }

        private void Configure(IReadOnlyCollection<PackageModelPersistable> packages, ImportConfig config, OpenApiDocument document)
        {
            _config = config;
            _metadataLookup = new MetadataLookup(packages);
            if (!_metadataLookup.TryGetTypeDefinitionByName("string", 0, out _fallbackType))
            {
                throw new Exception("Failed to find 'string' type definition in the provided packages. Ensure the package contains basic type definitions.");
            }
            _metadataFactory = GetMetadataCreationFactory(document);
        }

        private IServiceCreationStrategy GetMetadataCreationFactory(OpenApiDocument document)
        {
            return _config.ServiceType switch
            {
                ServiceType.Service => new ServiceServiceCreationStrategy(_metadataLookup, _config, this),
                ServiceType.CQRS => new CQRSServiceCreationStrategy(_metadataLookup, _config, this),
                _ => throw new Exception("Unknown Service Type configuration")
            };
        }

        private class OperationDetails
        {
            private bool _conceptIsPlural = false;

            public OperationDetails(string serviceRoute, string operationRoute, string serviceName, List<string> conceptName, string restType, OpenApiOperation operation)
            {
                ServiceRoute = serviceRoute;
                OperationRoute = operationRoute;
                ServiceName = serviceName;
                if (!conceptName.Any())
                {
                    conceptName.Add(serviceName);
                }

                conceptName[conceptName.Count - 1] = conceptName[conceptName.Count - 1].Singularize(false);
                ConceptName = conceptName;
                if (restType == "get" && !operationRoute.EndsWith("}"))
                {
                    PluralizeOperationName();
                }

                OperationName = GetOperationName(string.Join(string.Empty, ConceptName), restType);
                RestType = restType;
                Operation = operation;
            }

            public string ServiceRoute { get; }
            public string OperationRoute { get; }
            public string ServiceName { get; }
            public string OperationName { get; internal set; }
            public List<string> ConceptName { get; }
            public string RestType { get; }
            public OpenApiOperation Operation { get; }

            internal void PluralizeOperationName()
            {
                if (!_conceptIsPlural)
                {
                    ConceptName[ConceptName.Count - 1] = ConceptName[ConceptName.Count - 1].Pluralize(true);
                    OperationName = GetOperationName(string.Join("", ConceptName), RestType);
                    _conceptIsPlural = true;
                }
            }

            private static string GetOperationName(string conceptName, string restType)
            {
                return restType switch
                {
                    "post" => conceptName.StartsWith("Create") ? conceptName : $"Create{conceptName}",
                    "put" => conceptName.StartsWith("Update") ? conceptName : $"Update{conceptName}",
                    "get" => conceptName.StartsWith("Get") ? conceptName : $"Get{conceptName.Replace("Find", "")}",
                    "delete" => conceptName.StartsWith("Delete") ? conceptName : $"Delete{conceptName}",
                    "patch" => conceptName.StartsWith("Update") ? conceptName : $"Update{conceptName}",
                    _ => conceptName
                };

            }
        }
    }
}
