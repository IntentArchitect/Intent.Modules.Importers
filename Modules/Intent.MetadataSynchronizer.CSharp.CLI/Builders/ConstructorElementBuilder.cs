using Intent.IArchitect.Agent.Persistence.Model;
using Intent.Modelers.Domain.Api;

namespace Intent.MetadataSynchronizer.CSharp.Importer.Builders;

public class ConstructorElementBuilder<TCtor> : ElementBuilderBase
{
    private readonly TCtor _dataSource;
    private readonly string _baseExternalReference;
    private readonly BuilderMetadataManager _builderMetadataManager;
    private readonly List<string> _paramTypes = new();

    public ConstructorElementBuilder(TCtor dataSource, string name, string baseExternalReference, BuilderMetadataManager builderMetadataManager)
    {
        _dataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentNullException(nameof(name));
        }

        _baseExternalReference = baseExternalReference ?? throw new ArgumentNullException(nameof(baseExternalReference));
        _builderMetadataManager = builderMetadataManager;
        _element = ElementPersistable.Create(
            specializationType: ClassConstructorModel.SpecializationType,
            specializationTypeId: ClassConstructorModel.SpecializationTypeId,
            name: name,
            parentId: null,
            externalReference: null);
    }

    public ConstructorElementBuilder<TCtor> AddParameter<TParam>(TParam dataSource, Func<TParam, ParameterBuilder> parameterBuilderFunc)
    {
        var builder = parameterBuilderFunc(dataSource);
        var result = builder.Build();
        _element.AddElement(result);
        _builderMetadataManager.SetTypeReferenceTypeId(result, builder.ParameterType);
        _paramTypes.Add(builder.ParameterType);
        return this;
    }

    public ConstructorElementBuilder<TCtor> AddParameters<TParam>(Func<TCtor, IEnumerable<TParam>> dataSource, Func<TParam, ParameterBuilder> parameterBuilderFunc)
    {
        foreach (var dataEntry in dataSource(_dataSource))
        {
            AddParameter(dataEntry, parameterBuilderFunc);
        }

        return this;
    }

    protected override void PerformIntermediateBuildStep()
    {
        _element.ExternalReference = $"Ctor+{string.Join("+", new[] { _baseExternalReference }.Concat(_paramTypes))}";
        foreach (var childElement in _element.ChildElements)
        {
            childElement.ExternalReference = $"{_element.ExternalReference}+{childElement.Name}";
        }
    }
}