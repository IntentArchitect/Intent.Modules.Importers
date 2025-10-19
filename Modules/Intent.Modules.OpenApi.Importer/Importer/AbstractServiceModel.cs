using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.OpenApi.Importer.Importer
{
    public class AbstractServiceOperationModel
    {
        public AbstractServiceOperationModel(string restType, string serviceRoute, string serviceName, string operationRoute, string operationName, string specializedConceptName, ResolvedType? bodyType, ResolvedType? returnType, List<Parameter> parameters, bool secured, string? restSuccessCode)
        {
            RestType = restType;
            ServiceRoute = serviceRoute;
            OperationRoute = operationRoute;
            OperationName = operationName;
            RootConceptName = serviceName; // Customer
            SpecializedConceptName = specializedConceptName; // Customer
            ServiceName = serviceName; //CustomerService
            ReturnType = returnType;
            Parameters = parameters;
            BodyType = bodyType;
            Secured = secured;
            RestSuccessCode = restSuccessCode;
        }

        public string RestType { get; set; }
        public string ServiceRoute { get; set; }
        public string OperationRoute { get; set; }
        public string OperationName { get; set; }
        public string ServiceName { get; set; }
        public string RootConceptName { get; set; }
        public string SpecializedConceptName { get; set; }
        public bool Secured { get; set; }
        public string? RestSuccessCode { get; set; }

        public ResolvedType? ReturnType { get; set; }
        public ResolvedType? BodyType { get; set; }
        public List<Parameter>? Parameters { get; set; }
    }

    public class Parameter
    {
        public Parameter(OpenApiParameter parameterInfo, ResolvedType resolvedType)
        {
            ParameterInfo = parameterInfo;
            ResolvedType = resolvedType;
        }

        public OpenApiParameter ParameterInfo { get; set; }
        public ResolvedType ResolvedType { get; set; }
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
                    return "enum";
                if (OpenApiType.AllOf != null && OpenApiType.AllOf.Any(x => x.Enum != null && x.Enum.Any()))
                    return "enum";
                if (OpenApiType.Type == null && OpenApiType.Reference == null)
                    return "object";
                if (OpenApiType.Type == "string" && OpenApiType.Format == "uuid")
                    return "uuid";
                return OpenApiType.Type ?? "object";
            }
        }

        public string TypeName { get; set; }
        public OpenApiSchema OpenApiType { get; set; }
        public bool IsCollection { get; set; }
        public bool IsNullable { get; set; }

        private static string GetNameFromRefId(string swaggerRefId)
        {
            //#/components/schemas/ImportServicesTest.Application.Customers.CreateCustomerAddressDto
            //#/components/schemas/CleanArchitecture.TestApplication.Application.Common.Pagination.PagedResult`1[[CleanArchitecture.TestApplication.Application.AggregateRootLongs.AggregateRootLongDto, CleanArchitecture.TestApplication.Application, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]
            int endPos = swaggerRefId.Length - 1;
            int genericPos = swaggerRefId.IndexOf('`');
            if (genericPos != -1) endPos = genericPos - 1;
            int startPos = swaggerRefId.LastIndexOf('.', endPos) + 1;

            return swaggerRefId.Substring(startPos, endPos - startPos + 1);
        }

    }

}
