using Intent.IArchitect.Agent.Persistence.Model;
using Intent.Modelers.Domain.Api;

namespace Intent.MetadataSynchronizer.CSharp.CLI.Builders;

public class ClassElementBuilder : ElementBuilderBase
{
    private readonly BuilderMetadataManager _builderMetadataManager;

    public ClassElementBuilder(string externalReference, string name, BuilderMetadataManager builderMetadataManager)
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
            specializationType: ClassModel.SpecializationType,
            specializationTypeId: ClassModel.SpecializationTypeId,
            name: name,
            parentId: null,
            externalReference: externalReference);
    }

    public ClassElementBuilder SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            ArgumentException.ThrowIfNullOrEmpty(nameof(name));
            return this;
        }

        _element.Name = name;
        return this;
    }

    public ClassElementBuilder SetParentId(string? parentId)
    {
        if (string.IsNullOrWhiteSpace(parentId))
        {
            ArgumentException.ThrowIfNullOrEmpty(nameof(parentId));
            return this;
        }

        _element.ParentFolderId = parentId;
        return this;
    }

    public ClassElementBuilder AddConstructor<TCtor>(TCtor dataSource, Action<ConstructorElementBuilder<TCtor>> constructorBuilderAction)
    {
        var builder = new ConstructorElementBuilder<TCtor>(dataSource, Name, ExternalReference, _builderMetadataManager);
        constructorBuilderAction(builder);
        var result = builder.Build();
        _element.AddElement(result);

        return this;
    }

    public ClassElementBuilder AddConstructors<TCtor>(IEnumerable<TCtor> dataSource, Action<ConstructorElementBuilder<TCtor>> constructorBuilderAction)
    {
        foreach (var dataEntry in dataSource)
        {
            AddConstructor(dataEntry, constructorBuilderAction);
        }

        return this;
    }

    public ClassElementBuilder AddAttribute<TAttribute>(TAttribute dataSource, Func<TAttribute, AttributeBuilder> attributeBuilderFunc)
    {
        var builder = attributeBuilderFunc(dataSource);
        var result = builder.Build();
        _builderMetadataManager.SetTypeReferenceTypeId(result, builder.AttributeType);
        _element.AddElement(result);
        return this;
    }

    public ClassElementBuilder AddAttributes<TAttribute>(IEnumerable<TAttribute> dataSource, Func<TAttribute, AttributeBuilder> attributeBuilderFunc)
    {
        foreach (var dataEntry in dataSource)
        {
            AddAttribute(dataEntry, attributeBuilderFunc);
        }

        return this;
    }

    public ClassElementBuilder AddOperation<TOperation>(TOperation dataSource, Func<TOperation, string> getNameFunc, Action<OperationElementBuilder<TOperation>> operationBuilderAction)
    {
        var builder = new OperationElementBuilder<TOperation>(dataSource, getNameFunc, ExternalReference, _builderMetadataManager);
        operationBuilderAction(builder);
        var result = builder.Build();
        _element.AddElement(result);
        return this;
    }

    public ClassElementBuilder AddOperations<TOperation>(IReadOnlyList<TOperation> dataSource, Func<TOperation, string> getNameFunc, Action<OperationElementBuilder<TOperation>> operationBuilderAction)
    {
        foreach (var dataEntry in dataSource)
        {
            AddOperation(dataEntry, getNameFunc, operationBuilderAction);
        }

        return this;
    }

    public ClassElementBuilder SetIsAbstract(bool isAbstract)
    {
        _element.IsAbstract = isAbstract;
        return this;
    }

    protected override void PerformIntermediateBuildStep()
    {
        foreach (var childElement in _element.ChildElements.Where(p => p.SpecializationTypeId == AttributeModel.SpecializationTypeId))
        {
            childElement.ExternalReference = $"{_element.ExternalReference}+{childElement.Name}";
        }
    }
}