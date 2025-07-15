using Intent.IArchitect.Agent.Persistence.Model;
using Intent.Modelers.Domain.Services.Api;

namespace Intent.MetadataSynchronizer.CSharp.CLI.Builders;

public class DomainServiceElementBuilder : ElementBuilderBase
{
    private readonly BuilderMetadataManager _builderMetadataManager;

    public DomainServiceElementBuilder(string externalReference, string name, BuilderMetadataManager builderMetadataManager)
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
            specializationType: DomainServiceModel.SpecializationType,
            specializationTypeId: DomainServiceModel.SpecializationTypeId,
            name: name,
            parentId: null,
            externalReference: externalReference);
    }

    public DomainServiceElementBuilder SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            ArgumentException.ThrowIfNullOrEmpty(nameof(name));
            return this;
        }

        _element.Name = name;
        return this;
    }

    public DomainServiceElementBuilder SetParentId(string? parentId)
    {
        if (string.IsNullOrWhiteSpace(parentId))
        {
            ArgumentException.ThrowIfNullOrEmpty(nameof(parentId));
            return this;
        }

        _element.ParentFolderId = parentId;
        return this;
    }

    public DomainServiceElementBuilder AddOperation<TOperation>(TOperation dataSource, Func<TOperation, string> getNameFunc, Action<OperationElementBuilder<TOperation>> operationBuilderAction)
    {
        var builder = new OperationElementBuilder<TOperation>(dataSource, getNameFunc, ExternalReference, _builderMetadataManager);
        operationBuilderAction(builder);
        var result = builder.Build();
        _element.AddElement(result);
        return this;
    }

    public DomainServiceElementBuilder AddOperations<TOperation>(IReadOnlyList<TOperation> dataSource, Func<TOperation, string> getNameFunc, Action<OperationElementBuilder<TOperation>> operationBuilderAction)
    {
        foreach (var dataEntry in dataSource)
        {
            AddOperation(dataEntry, getNameFunc, operationBuilderAction);
        }

        return this;
    }

    protected override void PerformIntermediateBuildStep()
    {

    }
}