using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.MetadataSynchronizer;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using OperationModel = Intent.Modelers.Services.Api.OperationModel;
using ParameterModel = Intent.Modelers.Services.Api.ParameterModel;

namespace Intent.Modules.OpenApi.Importer.Importer.ServiceCreation
{
    internal class ServiceServiceCreationStrategy : ServiceCreationStrategyBase, IServiceCreationStrategy
    {
        private protected readonly Dictionary<string, ElementPersistable> _services;

        public ServiceServiceCreationStrategy(MetadataLookup metadataLookup, ImportConfig config, IOpenApiPresistableFactory openApiHelper)
            : base(metadataLookup, config, openApiHelper)
        {
            _services = new Dictionary<string, ElementPersistable>();
        }

        protected ElementPersistable GetOrCreateService(AbstractServiceOperationModel serviceDefinition)
        {
            string serviceName = serviceDefinition.ServiceName.ToPascalCase();
            string key = $"Service:{serviceName}";
            if (!_services.TryGetValue(key, out var service))
            {
                service = ElementPersistable.Create(
                    specializationType: ServiceModel.SpecializationType,
                    specializationTypeId: ServiceModel.SpecializationTypeId,
                    name: serviceName,
                    parentId: null,
                    externalReference: serviceName);
                _services.Add(key, service);
                _addedServiceTypes.Add(key, service);

                service.Stereotypes.Add(
                    CreateHttpServiceSettingsStereotype(
                        serviceDefinition.ServiceRoute
                        ));
            }
            return service;
        }

        protected ElementPersistable CreateOperation(string serviceName, string operationName, string parentId)
        {

            string key = $"Operation:{serviceName}:{operationName}";
            var operation = ElementPersistable.Create(
                    specializationType: OperationModel.SpecializationType,
                    specializationTypeId: OperationModel.SpecializationTypeId,
                    name: operationName.ToPascalCase(),
                    parentId: parentId,
                    externalReference: $"{serviceName.ToPascalCase()}.{operationName.ToPascalCase()}");
            _addedServiceTypes.Add(key, operation);
            return operation;
        }

        protected override Persistables CreateServicesInternal(IList<AbstractServiceOperationModel> serviceDefinitions)
        {
            var associations = new List<AssociationPersistable>();

            foreach (var serviceDefinition in serviceDefinitions)
            {
                if (Config.AddPostFixes && !serviceDefinition.ServiceName.EndsWith("Service"))
                {
                    serviceDefinition.ServiceName += "Service";
                }

                var folder = GetOrCreateFolder(serviceDefinition.RootConceptName);
                var serviceElement = GetOrCreateService(serviceDefinition);
                var operationElement = CreateOperation(serviceDefinition.ServiceName, serviceDefinition.OperationName, serviceElement.Id);

                if (serviceDefinition.BodyType != null)
                {
                    var bodyElement = CreateOrGetDto(type: serviceDefinition.BodyType,
                        specializationType: DTOModel.SpecializationType,
                        specializationTypeId: DTOModel.SpecializationTypeId,
                        folder: folder 
                        );

                    var propertyElement = ElementPersistable.Create(
                        specializationType: ParameterModel.SpecializationType,
                        specializationTypeId: ParameterModel.SpecializationTypeId,
                        name: "dto",
                        parentId: operationElement.Id,
                        externalReference: $"{serviceElement.Name}.{operationElement.Name}.dto");

                    _addedServiceTypes.Add(propertyElement.ExternalReference, propertyElement);

                    propertyElement.TypeReference = TypeReferencePersistable.Create(
                        typeId: bodyElement.Id,
                        isNavigable: true,
                        isNullable: serviceDefinition.BodyType.IsNullable,
                        isCollection: serviceDefinition.BodyType.IsCollection,
                        isRequired: default,
                        comment: default,
                        genericTypeId: default,
                        typePackageName: bodyElement.PackageName,
                        typePackageId: bodyElement.PackageId,
                        stereotypes: new List<StereotypePersistable>(),
                        genericTypeParameters: new List<TypeReferencePersistable>());

                    _domain.AddDomainObjectForDto(serviceDefinition.SpecializedConceptName, bodyElement, serviceDefinition.RootConceptName);
                }
                if (serviceDefinition.ReturnType != null)
                {
                    operationElement.TypeReference = GetIntentType(serviceDefinition.ReturnType, folder);
                }
                AddParameters(serviceDefinition, ParameterModel.SpecializationType, ParameterModel.SpecializationTypeId, operationElement, serviceDefinition.ServiceName, (p) => p.ToCamelCase());

                operationElement.Stereotypes.Add(
                    CreateHttpSettingsStereotype(
                        serviceDefinition.RestType.ToUpper(),
                        serviceDefinition.OperationRoute,
                        "Default",
                        serviceDefinition.RestSuccessCode
                        ));
                if (Config.IsAzureFunctions)
                {
                    operationElement.Stereotypes.Add(
                        CreateAzureFunctionStereotype());
                }

                EnsureAuthorizeStereotype(operationElement, serviceDefinition.Secured);
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

        private StereotypePersistable CreateHttpServiceSettingsStereotype(string serviceRoute)
        {
            return new StereotypePersistable
            {
                DefinitionId = "c29224ec-d473-4b95-ad4a-ec55c676c4fd",
                Name = "Http Service Settings",
                Comment = null,
                AddedByDefault = false,
                DefinitionPackageName = "Intent.Metadata.WebApi",
                DefinitionPackageId = "0011387a-b122-45d7-9cdb-8e21b315ab9f",
                Properties = new List<StereotypePropertyPersistable>()
                    {
                        new StereotypePropertyPersistable() { DefinitionId = "1e223bd0-7a72-435a-8741-a612d88e4a12", Name = "Route", Value = serviceRoute },
                    }
            };

        }
    }
}
