using Intent.IArchitect.Agent.Persistence.Model;
using Intent.Modelers.Domain.Repositories.Api;

namespace Intent.MetadataSynchronizer.CSharp.Importer.Builders;

public class DomainRepositoryBuilder : ElementBuilderBase
{
    private readonly BuilderMetadataManager _builderMetadataManager;

    public DomainRepositoryBuilder(string externalReference, string name, BuilderMetadataManager builderMetadataManager)
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
            specializationType: RepositoryModel.SpecializationType,
            specializationTypeId: RepositoryModel.SpecializationTypeId,
            name: name,
            parentId: null,
            externalReference: externalReference);
    }

    public DomainRepositoryBuilder SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            ArgumentException.ThrowIfNullOrEmpty(nameof(name));
            return this;
        }

        _element.Name = name;
        return this;
    }

    public DomainRepositoryBuilder SetParentId(string? parentId)
    {
        if (string.IsNullOrWhiteSpace(parentId))
        {
            ArgumentException.ThrowIfNullOrEmpty(nameof(parentId));
            return this;
        }

        _element.ParentFolderId = parentId;
        return this;
    }

    public DomainRepositoryBuilder AddOperation<TOperation>(TOperation dataSource, Func<TOperation, string> getNameFunc, Action<OperationElementBuilder<TOperation>> operationBuilderAction)
    {
        var builder = new OperationElementBuilder<TOperation>(dataSource, getNameFunc, ExternalReference, _builderMetadataManager);
        operationBuilderAction(builder);
        var result = builder.Build();
        _element.AddElement(result);
        return this;
    }

    public DomainRepositoryBuilder AddOperations<TOperation>(IReadOnlyList<TOperation> dataSource, Func<TOperation, string> getNameFunc, Action<OperationElementBuilder<TOperation>> operationBuilderAction)
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