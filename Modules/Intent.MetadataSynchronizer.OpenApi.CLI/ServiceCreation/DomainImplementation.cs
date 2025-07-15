using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Microsoft.OpenApi.Models;
using SharpYaml.Tokens;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AttributeModel = Intent.Modelers.Domain.Api.AttributeModel;

namespace Intent.MetadataSynchronizer.OpenApi.CLI.ServiceCreation
{
    internal class DomainImplementation
    {
        private readonly Dictionary<string, ElementPersistable> _serviceTypes;
        private protected readonly MetadataLookup _serviceMetadataLookup;
        private protected ImportConfig _config;
        private Dictionary<string, (ElementPersistable Dto, ElementPersistable Entity)> _aggregateDtoLookup;
        private protected readonly Dictionary<string, ElementPersistable> _folders;
        internal Dictionary<string, ElementPersistable> _enumLookup;
        internal Dictionary<string, ElementPersistable> _dtoLookup;

        public DomainImplementation(MetadataLookup metadataLookup, ImportConfig config, Dictionary<string, ElementPersistable> serviceTypes)
        {
            DomainTypes = new Dictionary<string, ElementPersistable>();
            DomainAssociations = new Dictionary<string, AssociationPersistable>();
            _aggregateDtoLookup = new Dictionary<string, (ElementPersistable Dto, ElementPersistable Entity)>();
            _serviceMetadataLookup = metadataLookup;
            _config = config;
            _serviceTypes = serviceTypes;
            _folders = new Dictionary<string, ElementPersistable>();
            _enumLookup = new Dictionary<string, ElementPersistable>();
            _dtoLookup = new Dictionary<string, ElementPersistable>();
        }

        internal Dictionary<string, ElementPersistable> DomainTypes;
        internal Dictionary<string, AssociationPersistable> DomainAssociations;

        public void AddDomainObjectForDto(string conceptName, ElementPersistable dto, string folder)
        {
            GetOrCreateAggregate(conceptName, dto, folder);
        }

        public Persistables GetDomainPersistables()
        {
            PopulateDomain();
            return new Persistables(DomainTypes.Values, DomainAssociations.Values);
        }

        private class DtoLookup
        { 
            public ElementPersistable? Dto { get; set; }
            public List<ElementPersistable> Children { get; set; }
        }


        private void PopulateDomain()
        {
            
            var dtoChildLookup = new Dictionary<string, DtoLookup>();
            foreach (var dto in _serviceTypes.Values.Where(x => x.SpecializationType == CommandModel.SpecializationType || x.SpecializationType == QueryModel.SpecializationType || x.SpecializationType == DTOModel.SpecializationType || x.SpecializationType == DTOFieldModel.SpecializationType))
            {
                _dtoLookup[dto.Id] = dto;
                if (dtoChildLookup.TryGetValue(dto.Id, out var existing))
                { 
                    dtoChildLookup[dto.Id].Dto =  dto;
                }
                else
                {
                    dtoChildLookup.Add(dto.Id, new DtoLookup { Dto  = dto, Children = new List<ElementPersistable>() });
                }
                if (dto.ParentFolderId != null && dto.ParentFolderId != _config.PackageId)
                {
                    if (!dtoChildLookup.TryGetValue(dto.ParentFolderId, out var existingParent))
                    {
                        existingParent = new DtoLookup() { Children = new List<ElementPersistable>() };
                        dtoChildLookup[dto.ParentFolderId] = existingParent;
                    }
                    existingParent.Children.Add(dto);
                }
            }

            foreach (var kvp in _aggregateDtoLookup)
            {
                var lookup = dtoChildLookup[kvp.Value.Dto.Id];
                foreach (var child in lookup.Children)
                {
                    AddEntityAttribute(kvp.Value.Entity, child, dtoChildLookup, $"{kvp.Value.Entity.ParentFolderId}.{kvp.Value.Entity.Name}.{child.Name}");
                }
            }
        }

        private void AddEntityAttribute(ElementPersistable aggregate, ElementPersistable dtoField, Dictionary<string, DtoLookup> dtoChildLookup, string path)
        {
            if (!_config.ReverseEngineerImplementation)
            {
                return;
            }

            string name = SanitizeTypeName(dtoField.Name);
            string key = path;

            if (dtoField.TypeReference.TypePackageId == "870ad967-cbd4-4ea9-b86d-9c3a5d55ea67") // Intent.Common.Type - Primitive Types
            {
                AddAttribute(name, aggregate, key, CreateTypeReference(dtoField.TypeReference));
            }
            else
            {
                if (_enumLookup.TryGetValue(dtoField.TypeReference.TypeId, out var enumType))
                {
                    AddAttribute(name, aggregate, key, CreateTypeReference(dtoField.TypeReference));
                }
                else if (_dtoLookup.TryGetValue(dtoField.TypeReference.TypeId, out var dtoType))
                {
                    var entity = GetOrCreateEntity(dtoType.Name, dtoType, aggregate.ParentFolderId, dtoChildLookup, path);

                    string assoicationKey = path;
                    if (!DomainAssociations.ContainsKey(assoicationKey))
                    {
                        var association = new AssociationPersistable
                        {
                            Id = Guid.NewGuid().ToString().ToLower(),
                            SourceEnd = new AssociationEndPersistable
                            {
                                SpecializationType = "Association Source End", // https://dev.azure.com/intentarchitect/Intent%20Architect/_workitems/edit/584
                                SpecializationTypeId = AssociationSourceEndModel.SpecializationTypeId,
                                Name = aggregate.Name,
                                TypeReference = TypeReferencePersistable.Create(
                                        typeId: aggregate.Id,
                                        isNavigable: false,
                                        isNullable: false,
                                        isCollection: false,
                                        isRequired: default,
                                        comment: default,
                                        genericTypeId: default,
                                        typePackageName: default,
                                        typePackageId: default,
                                        stereotypes: new List<StereotypePersistable>(),
                                        genericTypeParameters: new List<TypeReferencePersistable>()),
                            },
                            TargetEnd = new AssociationEndPersistable
                            {
                                SpecializationType = "Association Target End", // https://dev.azure.com/intentarchitect/Intent%20Architect/_workitems/edit/584
                                SpecializationTypeId = AssociationTargetEndModel.SpecializationTypeId,
                                Name = dtoField.Name,
                                TypeReference = TypeReferencePersistable.Create(
                                        typeId: entity.Id,
                                        isNavigable: true,
                                        isNullable: dtoField.TypeReference.IsNullable,
                                        isCollection: dtoField.TypeReference.IsCollection,
                                        isRequired: default,
                                        comment: default,
                                        genericTypeId: default,
                                        typePackageName: default,
                                        typePackageId: default,
                                        stereotypes: new List<StereotypePersistable>(),
                                        genericTypeParameters: new List<TypeReferencePersistable>()),
                            },
                            AssociationType = AssociationModel.SpecializationType,
                            AssociationTypeId = AssociationModel.SpecializationTypeId
                        };

                        DomainAssociations.Add(assoicationKey, association);
                    }
                }
            }
        }

        private void AddAttribute(string name, ElementPersistable aggregate, string key, TypeReferencePersistable typeRef)
        {
            var attribute = ElementPersistable.Create(
                    specializationType: AttributeModel.SpecializationType,
                    specializationTypeId: AttributeModel.SpecializationTypeId,
                    name: name,
                    parentId: aggregate.Id,
                    externalReference: key);

            attribute.TypeReference = typeRef;
            DomainTypes.Add(key, attribute);
        }

        private TypeReferencePersistable CreateTypeReference(TypeReferencePersistable from, string? typeId = null)
        {
            return TypeReferencePersistable.Create(
                typeId: typeId ?? from.TypeId,
                isNavigable: from.IsNavigable,
                isNullable: from.IsNullable,
                isCollection: from.IsCollection,
                isRequired: from.IsRequired,
                comment: default,
                genericTypeId: default,
                typePackageName: from.TypePackageName,
                typePackageId: from.TypePackageId,
                stereotypes: [],
                genericTypeParameters: []);

        }

        protected ElementPersistable GetOrCreateAggregate(string conceptName, ElementPersistable dto, string folder)
        {
            var name = SanitizeTypeName(conceptName);
            var key = string.IsNullOrWhiteSpace(folder) ? name : $"{folder}->{name}" ;

            if (DomainTypes.TryGetValue(key, out var result))
            {
                return result;
            }

            string parentId = null;
            if (!string.IsNullOrWhiteSpace(folder))
            {
                parentId = GetOrCreateFolder(folder).Id;
            }

            result = ElementPersistable.Create(
                specializationType: ClassModel.SpecializationType,
                specializationTypeId: ClassModel.SpecializationTypeId,
                name: name,
                parentId: parentId,
                externalReference: key);

#warning I Need a IF Cosmos
            result.Stereotypes.Add(CreateContainerStereotype(conceptName));

            DomainTypes.Add(key, result);

            _aggregateDtoLookup.Add(result.Id, new(dto, result));

            return result;
        }


        private ElementPersistable GetOrCreateEntity(string conceptName, ElementPersistable dto, string? parentFolderId, Dictionary<string, DtoLookup> dtoChildLookup, string path)
        {
            var name = SanitizeTypeName(conceptName);
            var key = string.IsNullOrWhiteSpace(parentFolderId) ? name : $"{parentFolderId}->{name}";


            if (DomainTypes.TryGetValue(key, out var result))
            {
                return result;
            }

            result = ElementPersistable.Create(
                specializationType: ClassModel.SpecializationType,
                specializationTypeId: ClassModel.SpecializationTypeId,
                name: SanitizeTypeName(conceptName),
                parentId: parentFolderId,
                externalReference: key);

            DomainTypes.Add(key, result);

            var lookup = dtoChildLookup[dto.Id];
            foreach (var child in lookup.Children)
            {
                AddEntityAttribute(result, child, dtoChildLookup, $"{path}.{child.Name}");
            }

            return result;
        }

        protected static StereotypePersistable CreateContainerStereotype(string containerName)
        {
            return new StereotypePersistable
            {
                DefinitionId = "ef9b1772-18e1-44ad-b606-66406221c805",
                Name = "Container",
                Comment = null,
                AddedByDefault = false,
                DefinitionPackageName = "Intent.CosmosDB",
                DefinitionPackageId = "b52e75f6-d86b-4c53-b4bd-8a8c2c267865",
                Properties =
                [
                    new StereotypePropertyPersistable { DefinitionId = "7ebc3aff-936c-465b-ac34-3e52362090ef", Name = "Name", Value = containerName },
                ]
            };
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

        internal void AddEnums(IEnumerable<ElementPersistable> toMove)
        {
            foreach (var x in toMove)
            {
                var folderName = x.ExternalReference.Substring(0, x.ExternalReference.IndexOf("."));

                var folder = GetOrCreateFolder(folderName);

                if (x.SpecializationTypeId == EnumModel.SpecializationTypeId)
                {
                    x.ParentFolderId = folder.Id;
                }
                _enumLookup.Add(x.Id, x);
                DomainTypes.Add(x.ExternalReference, x);
            }
        }

        protected ElementPersistable GetOrCreateFolder(string conceptName)
        {
            var key = $"Folder:{conceptName.Pluralize().ToPascalCase()}";
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
            DomainTypes.Add(key, folder);

            return folder;
        }

    }
}
