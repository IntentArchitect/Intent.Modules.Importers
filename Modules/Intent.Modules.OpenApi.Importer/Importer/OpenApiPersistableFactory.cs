using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.MetadataSynchronizer;
using Intent.Modules.Common.Templates;
using Intent.Modules.OpenApi.Importer.Importer.ServiceCreation;
using Intent.Utils;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using Microsoft.OpenApi.Validations;

namespace Intent.Modules.OpenApi.Importer.Importer;

public class OpenApiPersistableFactory : IOpenApiPersistableFactory
{
    private readonly Dictionary<string, string> _openApiTypeMapping = new();
    private readonly Dictionary<string, ElementPersistable> _intentTypeLookup = new();
    private Dictionary<string, OpenApiSchema> _swaggerComponentsLookup = null!;
    private ElementPersistable _fallbackType = null!;
    private MetadataLookup _metadataLookup = null!;
    private OpenApiImportConfig _config = null!;
    private IServiceCreationStrategy _metadataFactory = null!;
    private OpenApiDocument _document = null!;
    private readonly List<string> _warnings = new();

    public OpenApiPersistableFactory()
    {
        LoadOpenApiTypeMapping();
    }

    public IReadOnlyCollection<string> Warnings => _warnings;

    public ElementPersistable GetIntentType(string openApiType)
    {
        if (!_intentTypeLookup.TryGetValue(openApiType, out var intentType))
        {
            if (_openApiTypeMapping.TryGetValue(openApiType, out var mappedType))
            {
                if (!_metadataLookup.TryGetTypeDefinitionByName(mappedType, 0, out intentType))
                {
                    throw new Exception($"Unknown Intent Type : '{mappedType}'");
                }
            }
            else if (!_metadataLookup.TryGetTypeDefinitionByName(openApiType, 0, out intentType))
            {
                intentType = _fallbackType;
            }

            _intentTypeLookup.Add(openApiType, intentType);
        }

        return intentType;
    }

    public Persistables GetPersistables(OpenApiImportConfig config, IReadOnlyCollection<PackageModelPersistable> packages)
    {
        using var stream = GetRelevantStream(config.OpenApiSpecificationFile);
        return GetPersistables(stream, config, packages);
    }

    public Persistables GetPersistables(Stream stream, OpenApiImportConfig config, IReadOnlyCollection<PackageModelPersistable> packages)
    {
        var document = LoadDocument(stream);
        _document = document;

        Configure(packages, config, document);

        var services = ParseServices(config, document);
        return _metadataFactory.CreateServices(services);
    }

    public void PersistAdditionalMetadata(PackageModelPersistable package)
    {
        if (_config.SettingPersistence != SettingPersistence.None)
        {
            package.AddMetadata("open-api-import:open-api-file", _config.OpenApiSpecificationFile);
            package.AddMetadata("open-api-import:add-postfixes", _config.AddPostFixes.ToString().ToLowerInvariant());
            package.AddMetadata("open-api-import:allow-removal", _config.AllowRemoval.ToString().ToLowerInvariant());
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

    public ResolvedType Resolve(OpenApiSchema schema, string? typeNameContext = null, bool impliedNullables = false, ISet<string>? requiredProperties = null)
    {
        var isCollection = schema.Type == "array";
        if (isCollection)
        {
            schema = schema.Items;
        }

        var response = schema;
        if (response.Reference != null)
        {
            response = _swaggerComponentsLookup[response.Reference.Id];
        }

        var nullable = !isCollection && (response.Nullable || response.Enum.Any(l => l is OpenApiNull));

        if (impliedNullables && requiredProperties != null && typeNameContext != null && !requiredProperties.Contains(typeNameContext))
        {
            nullable = true;
        }

        return new ResolvedType(response, isCollection, nullable, typeNameContext ?? "Anonymous");
    }

    public bool IsNull(OpenApiSchema schema)
    {
        return schema == null || (schema.Type == null && schema.Reference == null && schema.AllOf == null);
    }

    private IList<AbstractServiceOperationModel> ParseServices(OpenApiImportConfig config, OpenApiDocument document)
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
            var url = path.Key;
            foreach (var operationKvp in path.Value.Operations)
            {
                var restType = operationKvp.Key.ToString().ToLowerInvariant();
                var operation = operationKvp.Value;

                var operationDetails = GetServiceDetails(restType, url, operation);
                operations.Add(operationDetails);

                //May need to adjust the name based on the response type
                if (TryGetResponseType(operation, operationDetails, out var response ))
                {
                    if (response.IsCollection)
                    {
                        operationDetails.PluralizeOperationName();
                    }
                }
            }
        }

        var duplicates = operations
            .GroupBy(o => new { o.ServiceName, o.OperationName })
            .Where(g => g.Count() > 1)
            .SelectMany(g => g.Select((op, index) => new { Operation = op, Index = index }))
            .ToList();

        foreach (var item in duplicates.Where(item => item.Index > 0))
        {
            item.Operation.OperationName += item.Index;
        }

        foreach (var operationDetails in operations)
        {
            var operation = operationDetails.Operation;
            TryGetResponseType(operation, operationDetails, out var response);

            string? restSuccessCode = null;
            if (operation.Responses.Count(r => r.Key.StartsWith("2", StringComparison.Ordinal)) == 1)
            {
                restSuccessCode = operation.Responses.First(r => r.Key.StartsWith("2", StringComparison.Ordinal)).Key.Trim();
            }

            ResolvedType? bodyType = null;
            if (operation.RequestBody?.Content.Count > 0)
            {
                if (!operation.RequestBody.Content.TryGetValue("application/json", out var content))
                {
                    content = operation.RequestBody.Content.FirstOrDefault().Value;
                }

                if (!IsNull(content.Schema))
                {
                    bodyType = Resolve(content.Schema, operationDetails.OperationName);
                }
            }

            var parameters = new List<Parameter>();
            foreach (var parameter in operation.Parameters)
            {
                parameters.Add(new Parameter(parameter, Resolve(parameter.Schema, parameter.Name)));
            }

            var secured = operation.Security != null && operation.Security.Any();
            result.Add(new AbstractServiceOperationModel(
                operationDetails.RestType,
                operationDetails.ServiceRoute,
                operationDetails.ServiceName,
                operationDetails.OperationRoute,
                operationDetails.OperationName,
                operationDetails.ConceptName.LastOrDefault() ?? operationDetails.ServiceName,
                bodyType,
                response,
                parameters,
                secured,
                restSuccessCode));
        }

        return result;
    }

    private bool TryGetResponseType(OpenApiOperation operation, OperationDetails operationDetails, out ResolvedType response)
    {
        var successResponses = operation.Responses.FirstOrDefault(r => r.Key.StartsWith("2", StringComparison.Ordinal)).Value;
        if (successResponses == null)
        {
            operation.Responses.TryGetValue("default", out successResponses);
        }
        if (successResponses?.Content != null && successResponses.Content.Count > 0)
        {
            if (!successResponses.Content.TryGetValue("application/json", out var content))
            {
                content = successResponses.Content.FirstOrDefault().Value;
            }

            if (!IsNull(content.Schema))
            {
                response = Resolve(content.Schema, operationDetails.OperationName);
                return true;
            }
        }
        response = null;
        return false;
    }

    private void Configure(IReadOnlyCollection<PackageModelPersistable> packages, OpenApiImportConfig config, OpenApiDocument document)
    {
        _warnings.Clear();
        _config = config;
        _metadataLookup = new MetadataLookup(packages);
        if (!_metadataLookup.TryGetTypeDefinitionByName("string", 0, out _fallbackType))
        {
            throw new Exception("Unable to resolve primitive type 'string' from metadata");
        }

        _metadataFactory = config.ServiceType switch
        {
            ServiceType.Service => new ServiceServiceCreationStrategy(_metadataLookup, config, this, _warnings),
            ServiceType.CQRS => new CQRSServiceCreationStrategy(_metadataLookup, config, this, _warnings),
            _ => throw new Exception("Unknown Service Type configuration")
        };
    }

    private OperationDetails GetServiceDetails(string restType, string url, OpenApiOperation operation)
    {
        var serviceRoute = string.Empty;
        var operationRoute = string.Empty;
        var serviceName = string.Empty;
        var conceptName = new List<string>();
        var split = url.Split('/');
        for (var i = 0; i < split.Length; i++)
        {
            if (serviceName == string.Empty)
            {
                serviceRoute = string.Join("/", split.Take(i + 1));
                operationRoute = string.Join("/", split.Skip(i + 1));
                if (!split[i].Equals("api", StringComparison.OrdinalIgnoreCase))
                {
                    serviceName = split[i].Singularize().ToPascalCase();
                }
            }
            else if (!split[i].Contains("{", StringComparison.Ordinal))
            {
                conceptName.Add(split[i].ToPascalCase());
            }
        }

        return new OperationDetails(
            serviceRoute: serviceRoute,
            operationRoute: operationRoute,
            serviceName: serviceName,
            conceptName: conceptName,
            restType: restType,
            operation: operation);
    }

    private string GetOpenApiDocumentUrl()
    {
        var httpsServer = _document.Servers.FirstOrDefault(s => s.Url.StartsWith("https", StringComparison.OrdinalIgnoreCase));
        if (httpsServer is not null && !string.IsNullOrWhiteSpace(httpsServer.Url))
        {
            return httpsServer.Url;
        }

        var httpServer = _document.Servers.FirstOrDefault(s => s.Url.StartsWith("http", StringComparison.OrdinalIgnoreCase));
        if (httpServer is not null && !string.IsNullOrWhiteSpace(httpServer.Url))
        {
            return httpServer.Url;
        }

        if (Uri.TryCreate(_config.OpenApiSpecificationFile, UriKind.Absolute, out var uri) &&
            (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps) &&
            _document.Servers.Any(s => !string.IsNullOrWhiteSpace(s.Url)))
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
        if (Uri.TryCreate(sourceFile, UriKind.Absolute, out var uri))
        {
            switch (uri.Scheme)
            {
                case "http":
                case "https":
                {
                    using var client = new HttpClient();
                    var httpContent = client.GetAsync(sourceFile).Result;
                    httpContent.EnsureSuccessStatusCode();
                    var memoryStream = new MemoryStream();
                    httpContent.Content.CopyToAsync(memoryStream).Wait();
                    memoryStream.Position = 0;
                    return memoryStream;
                }
                case "file":
                    if (!File.Exists(sourceFile))
                    {
                        throw new Exception($"Unable to find file : {sourceFile}");
                    }

                    return new FileStream(sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            }
        }

        if (!File.Exists(sourceFile))
        {
            throw new Exception($"Unable to find file : {sourceFile}");
        }

        return new FileStream(sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read);
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

    private class OperationDetails
    {
        private bool _conceptIsPlural;

        public OperationDetails(string serviceRoute, string operationRoute, string serviceName, List<string> conceptName, string restType, OpenApiOperation operation)
        {
            ServiceRoute = serviceRoute;
            OperationRoute = operationRoute;
            ServiceName = serviceName;
            if (!conceptName.Any())
            {
                conceptName.Add(serviceName);
            }

            conceptName[^1] = conceptName[^1].Singularize(false);
            ConceptName = conceptName;
            if (restType == "get" && !operationRoute.EndsWith("}", StringComparison.Ordinal))
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
            if (_conceptIsPlural)
            {
                return;
            }

            ConceptName[^1] = ConceptName[^1].Pluralize(true);
            OperationName = GetOperationName(string.Join(string.Empty, ConceptName), RestType);
            _conceptIsPlural = true;
        }

        private static string GetOperationName(string conceptName, string restType)
        {
            return restType switch
            {
                "post" => conceptName.StartsWith("Create", StringComparison.OrdinalIgnoreCase) ? conceptName : $"Create{conceptName}",
                "put" => conceptName.StartsWith("Update", StringComparison.OrdinalIgnoreCase) ? conceptName : $"Update{conceptName}",
                "get" => conceptName.StartsWith("Get", StringComparison.OrdinalIgnoreCase) ? conceptName : $"Get{conceptName.Replace("Find", string.Empty, StringComparison.OrdinalIgnoreCase)}",
                "delete" => conceptName.StartsWith("Delete", StringComparison.OrdinalIgnoreCase) ? conceptName : $"Delete{conceptName}",
                "patch" => conceptName.StartsWith("Update", StringComparison.OrdinalIgnoreCase) ? conceptName : $"Update{conceptName}",
                _ => conceptName
            };
        }
    }
}
