using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Metadata;
using Intent.MetadataSynchronizer;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Utils;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.OpenApi.Importer.Importer.ServiceCreation
{
    internal class CQRSServiceCreationStrategy : ServiceCreationStrategyBase, IServiceCreationStrategy
    {

        public CQRSServiceCreationStrategy(MetadataLookup metadataLookup, ImportConfig config, IOpenApiPresistableFactory openApiHelper)
            : base(metadataLookup, config, openApiHelper)
        {
        }

        protected override Persistables CreateServicesInternal(IList<AbstractServiceOperationModel> serviceDefinitions)
        {           
            var associations = new List<AssociationPersistable>();

            string specializationType;
            string specializationTypeId;

            foreach (var serviceDefinition in serviceDefinitions)
            {
                if (serviceDefinition?.BodyType?.IsCollection == true)
                {
                    Logging.Log.Warning($"Unable to create endpoint for ({serviceDefinition.ServiceName}.{serviceDefinition.OperationName}) array based body can't be represented as a Command.");
                    continue;
                }

                switch (serviceDefinition.RestType)
                {
                    case "get":
                        specializationType = QueryModel.SpecializationType;
                        specializationTypeId = QueryModel.SpecializationTypeId;
                        if (Config.AddPostFixes && !serviceDefinition.OperationName.EndsWith("Query"))
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
                        if (Config.AddPostFixes && !serviceDefinition.OperationName.EndsWith("Command"))
                        {
                            serviceDefinition.OperationName += "Command";
                        }
                        break;
                }

                string serviceElementName = serviceDefinition.OperationName.ToPascalCase();
                var folder = GetOrCreateFolder(serviceDefinition.RootConceptName);
                ElementPersistable serviceElement;
                if (serviceDefinition.BodyType != null)
                {
                    serviceElement = CreateOrGetDto(type: serviceDefinition.BodyType,
                        specializationType: specializationType,
                        specializationTypeId: specializationTypeId,
                        name: serviceElementName,
                        folder: folder,
                        externalReference: $"{folder.Name}.{serviceElementName}"
                        );
                    _domain.AddDomainObjectForDto(serviceDefinition.SpecializedConceptName, serviceElement, serviceDefinition.RootConceptName);
                }
                else
                {
                    serviceElement = CreateOrGetDto(null,
                        specializationType: specializationType,
                        specializationTypeId: specializationTypeId,
                        name: serviceElementName,
                        folder: folder,
                        externalReference: $"{folder.Name}.{serviceElementName}"
                        );
                }

                if (serviceDefinition.ReturnType != null)
                {
                    serviceElement.TypeReference = GetIntentType(serviceDefinition.ReturnType, folder);
                }

                AddParameters(serviceDefinition, DTOFieldModel.SpecializationType, DTOFieldModel.SpecializationTypeId, serviceElement, folder.Name, (p) => p.ToPascalCase());
                
                serviceElement.Stereotypes.Add(
                    CreateHttpSettingsStereotype(
                        serviceDefinition.RestType.ToUpper(),
                        $"{serviceDefinition.ServiceRoute}/{serviceDefinition.OperationRoute}",
                        "Default",
                        serviceDefinition.RestSuccessCode
                        ));
                if (Config.IsAzureFunctions)
                {
                    serviceElement.Stereotypes.Add(CreateAzureFunctionStereotype());
                }

                EnsureAuthorizeStereotype(serviceElement, serviceDefinition.Secured);
            }

            return new Persistables(_addedServiceTypes.Values, associations);
        }

        private void EnsureAuthorizeStereotype(ElementPersistable element, bool secured)
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
}
