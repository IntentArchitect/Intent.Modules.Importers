using System;
using System.Collections.Generic;
using System.Linq;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.MetadataSynchronizer;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;

namespace Intent.Modules.OpenApi.Importer.Importer.ServiceCreation;

internal class CQRSServiceCreationStrategy : ServiceCreationStrategyBase
{
    public CQRSServiceCreationStrategy(
        MetadataLookup metadataLookup,
        OpenApiImportConfig config,
    IOpenApiPersistableFactory openApiHelper,
    List<string> warnings)
    : base(metadataLookup, config, openApiHelper, warnings)
    {
    }

    protected override Persistables CreateServicesInternal(IList<AbstractServiceOperationModel> serviceDefinitions)
    {
        var associations = new List<AssociationPersistable>();

        foreach (var serviceDefinition in serviceDefinitions)
        {
            if (serviceDefinition is null)
            {
                continue;
            }
            if (serviceDefinition.BodyType?.IsCollection == true)
            {
                AddWarning(
                    $"Unable to create endpoint for ({serviceDefinition.ServiceName}.{serviceDefinition.OperationName}) array based body can't be represented as a Command.");
                continue;
            }

            string specializationType;
            string specializationTypeId;

            switch (serviceDefinition.RestType)
            {
                case "get":
                    specializationType = QueryModel.SpecializationType;
                    specializationTypeId = QueryModel.SpecializationTypeId;
                    if (Config.AddPostFixes && !serviceDefinition.OperationName.EndsWith("Query", StringComparison.Ordinal))
                    {
                        serviceDefinition.OperationName += "Query";
                    }

                    break;
                case "delete":
                case "post":
                case "patch":
                case "put":
                default:
                    specializationType = CommandModel.SpecializationType;
                    specializationTypeId = CommandModel.SpecializationTypeId;
                    if (Config.AddPostFixes && !serviceDefinition.OperationName.EndsWith("Command", StringComparison.Ordinal))
                    {
                        serviceDefinition.OperationName += "Command";
                    }

                    break;
            }

            var serviceElementName = serviceDefinition.OperationName.ToPascalCase();
            var folder = GetOrCreateFolder(serviceDefinition.RootConceptName);
            ElementPersistable serviceElement;
            if (serviceDefinition.BodyType != null)
            {
                serviceElement = CreateOrGetDto(
                    type: serviceDefinition.BodyType,
                    specializationType: specializationType,
                    specializationTypeId: specializationTypeId,
                    name: serviceElementName,
                    folder: folder,
                    externalReference: $"{folder.Name}.{serviceElementName}");
            }
            else
            {
                serviceElement = CreateOrGetDto(
                    null,
                    specializationType: specializationType,
                    specializationTypeId: specializationTypeId,
                    name: serviceElementName,
                    folder: folder,
                    externalReference: $"{folder.Name}.{serviceElementName}");
            }

            if (serviceDefinition.ReturnType != null)
            {
                serviceElement.TypeReference = GetIntentType(serviceDefinition.ReturnType, folder);
            }
            else if (serviceDefinition.RestType == "get")
            {
                AddWarning(
                    $"Query '{serviceDefinition.OperationName}' has no return type defined. Queries should return data. Please add a response schema to the OpenAPI specification.");
            }

            AddParameters(
                serviceDefinition,
                DTOFieldModel.SpecializationType,
                DTOFieldModel.SpecializationTypeId,
                serviceElement,
                folder.Name,
                static p => p.ToPascalCase());

            serviceElement.Stereotypes.Add(
                CreateHttpSettingsStereotype(
                    serviceDefinition.RestType.ToUpperInvariant(),
                    $"{serviceDefinition.ServiceRoute}/{serviceDefinition.OperationRoute}",
                    "Default",
                    serviceDefinition.RestSuccessCode));

            if (Config.IsAzureFunctions)
            {
                serviceElement.Stereotypes.Add(CreateAzureFunctionStereotype());
            }

            EnsureAuthorizeStereotype(serviceElement, serviceDefinition.Secured);
        }

        return new Persistables(AddedServiceTypes.Values, associations);
    }

    private static void EnsureAuthorizeStereotype(ElementPersistable element, bool secured)
    {
        var existingStereoType = element.Stereotypes.FirstOrDefault(s => s.Name == "Secured");
        if (!secured && existingStereoType != null)
        {
            element.Stereotypes.Remove(existingStereoType);
        }
        else if (secured && existingStereoType == null)
        {
            var toAdd = new StereotypePersistable
            {
                DefinitionId = "a9eade71-1d56-4be7-a80c-81046c0c978b",
                Name = "Secured",
                Comment = null,
                AddedByDefault = false,
                DefinitionPackageName = "Intent.Metadata.Security",
                DefinitionPackageId = "a6fa1088-0064-43e3-a7fc-36c97b2b9285",
                Properties = new List<StereotypePropertyPersistable>()
            };
            element.Stereotypes.Add(toAdd);
        }
    }
}
