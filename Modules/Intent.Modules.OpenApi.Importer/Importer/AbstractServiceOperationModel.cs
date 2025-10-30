using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Models;

namespace Intent.Modules.OpenApi.Importer.Importer;

public class AbstractServiceOperationModel
{
    public AbstractServiceOperationModel(
        string restType,
        string serviceRoute,
        string serviceName,
        string operationRoute,
        string operationName,
        string specializedConceptName,
        ResolvedType? bodyType,
        ResolvedType? returnType,
        List<Parameter> parameters,
        bool secured,
        string? restSuccessCode)
    {
        RestType = restType;
        ServiceRoute = serviceRoute;
        OperationRoute = operationRoute;
        OperationName = operationName;
        RootConceptName = serviceName;
        SpecializedConceptName = specializedConceptName;
        ServiceName = serviceName;
        ReturnType = returnType;
        Parameters = parameters;
        BodyType = bodyType;
        Secured = secured;
        RestSuccessCode = restSuccessCode;
    }

    public string RestType { get; }
    public string ServiceRoute { get; }
    public string OperationRoute { get; }
    public string OperationName { get; internal set; }
    public string ServiceName { get; internal set; }
    public string RootConceptName { get; }
    public string SpecializedConceptName { get; }
    public bool Secured { get; }
    public string? RestSuccessCode { get; }
    public ResolvedType? ReturnType { get; }
    public ResolvedType? BodyType { get; }
    public List<Parameter> Parameters { get; }
}

public class Parameter
{
    public Parameter(OpenApiParameter parameterInfo, ResolvedType resolvedType)
    {
        ParameterInfo = parameterInfo;
        ResolvedType = resolvedType;
    }

    public OpenApiParameter ParameterInfo { get; }
    public ResolvedType ResolvedType { get; }
}

public class ResolvedType
{
    public ResolvedType(OpenApiSchema openApiType, bool isCollection, bool isNullable, string typeNameSuggestion)
    {
        OpenApiType = openApiType;
        IsCollection = isCollection;
        IsNullable = isNullable;
        if (OpenApiType.Reference?.Id != null)
        {
            TypeName = GetNameFromRefId(OpenApiType.Reference.Id);
        }
        else
        {
            TypeName = typeNameSuggestion + "Type";
        }
    }

    public string SwaggerType
    {
        get
        {
            if (OpenApiType.Enum != null && OpenApiType.Enum.Any())
            {
                return "enum";
            }

            if (OpenApiType.AllOf != null && OpenApiType.AllOf.Any(x => x.Enum != null && x.Enum.Any()))
            {
                return "enum";
            }

            if (OpenApiType.Type == null && OpenApiType.Reference == null)
            {
                return "object";
            }

            if (OpenApiType.Type == "string" && OpenApiType.Format == "uuid")
            {
                return "uuid";
            }

            return OpenApiType.Type ?? "object";
        }
    }

    public string TypeName { get; }
    public OpenApiSchema OpenApiType { get; }
    public bool IsCollection { get; }
    public bool IsNullable { get; }

    private static string GetNameFromRefId(string swaggerRefId)
    {
        var endPos = swaggerRefId.Length - 1;
        var genericPos = swaggerRefId.IndexOf('`');
        if (genericPos != -1)
        {
            endPos = genericPos - 1;
        }

        var startPos = swaggerRefId.LastIndexOf('.', endPos) + 1;
        return swaggerRefId.Substring(startPos, endPos - startPos + 1);
    }
}
