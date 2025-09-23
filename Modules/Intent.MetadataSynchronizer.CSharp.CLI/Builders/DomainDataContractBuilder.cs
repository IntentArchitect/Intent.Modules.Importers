using Intent.IArchitect.Agent.Persistence.Model;
using Intent.Modelers.Domain.Api;

namespace Intent.MetadataSynchronizer.CSharp.Importer.Builders;

public class DomainDataContractBuilder : ElementBuilderBase
{
    private readonly BuilderMetadataManager _builderMetadataManager;

    public DomainDataContractBuilder(string externalReference, string name, BuilderMetadataManager builderMetadataManager)
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
            specializationType: DataContractModel.SpecializationType,
            specializationTypeId: DataContractModel.SpecializationTypeId,
            name: name,
            parentId: null,
            externalReference: externalReference);
    }
    
    public DomainDataContractBuilder AddAttribute<TAttribute>(TAttribute dataSource, Func<TAttribute, AttributeBuilder> attributeBuilderFunc)
    {
        var builder = attributeBuilderFunc(dataSource);
        var result = builder.Build();
        _builderMetadataManager.SetTypeReferenceTypeId(result, builder.AttributeType);
        _element.AddElement(result);
        return this;
    }

    public DomainDataContractBuilder AddAttributes<TAttribute>(IEnumerable<TAttribute> dataSource, Func<TAttribute, AttributeBuilder> attributeBuilderFunc)
    {
        foreach (var dataEntry in dataSource)
        {
            AddAttribute(dataEntry, attributeBuilderFunc);
        }

        return this;
    }
    
    public DomainDataContractBuilder SetParentId(string? parentId)
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