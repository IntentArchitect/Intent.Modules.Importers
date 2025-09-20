using Intent.IArchitect.Agent.Persistence.Model;
using Intent.Modelers.Eventing.Api;
using Intent.Modelers.Services.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.MetadataSynchronizer.CSharp.Importer.Builders;
public class EventDtoElementBuilder : ElementBuilderBase
{
    private readonly BuilderMetadataManager _builderMetadataManager;

    public EventDtoElementBuilder(string externalReference, string name, BuilderMetadataManager builderMetadataManager)
    {
        _builderMetadataManager = builderMetadataManager;

        if (string.IsNullOrWhiteSpace(externalReference))
        {
            throw new ArgumentNullException(nameof(externalReference));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            ArgumentException.ThrowIfNullOrEmpty(nameof(name));
            return;
        }

        _element = ElementPersistable.Create(
           specializationType: EventingDTOModel.SpecializationType,
           specializationTypeId: EventingDTOModel.SpecializationTypeId,
           name: name,
           parentId: null,
           externalReference: externalReference);
    }

    public EventDtoElementBuilder SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            ArgumentException.ThrowIfNullOrEmpty(nameof(name));
            return this;
        }

        _element.Name = name;
        return this;
    }

    public EventDtoElementBuilder SetParentId(string? parentId)
    {
        if (string.IsNullOrWhiteSpace(parentId))
        {
            ArgumentException.ThrowIfNullOrEmpty(nameof(parentId));
            return this;
        }

        _element.ParentFolderId = parentId;
        return this;
    }

    public EventDtoElementBuilder AddField<TAttribute>(TAttribute dataSource, Func<TAttribute, EventDtoFieldBuilder> propertyBuilderFunc)
    {
        var builder = propertyBuilderFunc(dataSource);
        var result = builder.Build();
        _builderMetadataManager.SetTypeReferenceTypeId(result, builder.FieldType);
        _element.AddElement(result);
        return this;
    }

    public EventDtoElementBuilder AddFields<TAttribute>(IEnumerable<TAttribute> dataSource, Func<TAttribute, EventDtoFieldBuilder> propertyBuilderFunc)
    {
        foreach (var dataEntry in dataSource)
        {
            AddField(dataEntry, propertyBuilderFunc);
        }

        return this;
    }

    protected override void PerformIntermediateBuildStep()
    {
        foreach (var childElement in _element.ChildElements.Where(p => p.SpecializationTypeId == EventingDTOFieldModel.SpecializationTypeId))
        {
            childElement.ExternalReference = $"{_element.ExternalReference}+{childElement.Name}";
        }
    }

}
