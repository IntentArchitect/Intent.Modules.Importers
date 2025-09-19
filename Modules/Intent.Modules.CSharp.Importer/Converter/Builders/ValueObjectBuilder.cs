using Intent.IArchitect.Agent.Persistence.Model;
using Intent.Modelers.Domain.ValueObjects.Api;

namespace Intent.MetadataSynchronizer.CSharp.CLI.Builders;

public class ValueObjectBuilder : ElementBuilderBase
{
    private readonly BuilderMetadataManager _builderMetadataManager;

    public ValueObjectBuilder(string externalReference, string name, BuilderMetadataManager builderMetadataManager)
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
            specializationType: ValueObjectModel.SpecializationType,
            specializationTypeId: ValueObjectModel.SpecializationTypeId,
            name: name,
            parentId: null,
            externalReference: externalReference);
    }
    
    public ValueObjectBuilder AddAttribute<TAttribute>(TAttribute dataSource, Func<TAttribute, AttributeBuilder> attributeBuilderFunc)
    {
        var builder = attributeBuilderFunc(dataSource);
        var result = builder.Build();
        _builderMetadataManager.SetTypeReferenceTypeId(result, builder.AttributeType);
        _element.AddElement(result);
        return this;
    }

    public ValueObjectBuilder AddAttributes<TAttribute>(IEnumerable<TAttribute> dataSource, Func<TAttribute, AttributeBuilder> attributeBuilderFunc)
    {
        foreach (var dataEntry in dataSource)
        {
            AddAttribute(dataEntry, attributeBuilderFunc);
        }

        return this;
    }
    
    public ValueObjectBuilder SetParentId(string? parentId)
    {
        if (string.IsNullOrWhiteSpace(parentId))
        {
            ArgumentException.ThrowIfNullOrEmpty(nameof(parentId));
            return this;
        }

        _element.ParentFolderId = parentId;
        return this;
    }
}