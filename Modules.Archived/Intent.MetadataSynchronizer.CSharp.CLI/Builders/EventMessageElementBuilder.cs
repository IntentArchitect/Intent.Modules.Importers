using Intent.IArchitect.Agent.Persistence.Model;
using Intent.Modelers.Eventing.Api;
using Intent.Modelers.Services.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.MetadataSynchronizer.CSharp.Importer.Builders;
public class EventMessageElementBuilder : ElementBuilderBase
{
    private readonly BuilderMetadataManager _builderMetadataManager;

    public EventMessageElementBuilder(string externalReference, string name, BuilderMetadataManager builderMetadataManager)
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
           specializationType: MessageModel.SpecializationType,
           specializationTypeId: MessageModel.SpecializationTypeId,
           name: name,
           parentId: null,
           externalReference: externalReference);
    }

    public EventMessageElementBuilder SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            ArgumentException.ThrowIfNullOrEmpty(nameof(name));
            return this;
        }

        _element.Name = name;
        return this;
    }

    public EventMessageElementBuilder SetParentId(string? parentId)
    {
        if (string.IsNullOrWhiteSpace(parentId))
        {
            ArgumentException.ThrowIfNullOrEmpty(nameof(parentId));
            return this;
        }

        _element.ParentFolderId = parentId;
        return this;
    }

    public EventMessageElementBuilder AddProperty<TAttribute>(TAttribute dataSource, Func<TAttribute, PropertyBuilder> propertyBuilderFunc)
    {
        var builder = propertyBuilderFunc(dataSource);
        var result = builder.Build();
        _builderMetadataManager.SetTypeReferenceTypeId(result, builder.PropertyType);
        _element.AddElement(result);
        return this;
    }

    public EventMessageElementBuilder AddProperties<TAttribute>(IEnumerable<TAttribute> dataSource, Func<TAttribute, PropertyBuilder> propertyBuilderFunc)
    {
        foreach (var dataEntry in dataSource)
        {
            AddProperty(dataEntry, propertyBuilderFunc);
        }

        return this;
    }

    protected override void PerformIntermediateBuildStep()
    {
        foreach (var childElement in _element.ChildElements.Where(p => p.SpecializationTypeId == PropertyModel.SpecializationTypeId))
        {
            childElement.ExternalReference = $"{_element.ExternalReference}+{childElement.Name}";
        }
    }

}
