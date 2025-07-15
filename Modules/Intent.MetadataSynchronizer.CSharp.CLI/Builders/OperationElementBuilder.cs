using Intent.IArchitect.Agent.Persistence.Model;
using Intent.Modelers.Domain.Api;

namespace Intent.MetadataSynchronizer.CSharp.CLI.Builders;

public class OperationElementBuilder<TOperation> : ElementBuilderBase
{
    private readonly TOperation _dataSource;
    private readonly string _baseExternalReference;
    private readonly BuilderMetadataManager _builderMetadataManager;
    private readonly List<string> _paramTypes = new();

    public OperationElementBuilder(TOperation dataSource, Func<TOperation, string> getNameFunc, string baseExternalReference, BuilderMetadataManager builderMetadataManager)
    {
        _dataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));

        _baseExternalReference = baseExternalReference ?? throw new ArgumentNullException(nameof(baseExternalReference));
        _builderMetadataManager = builderMetadataManager;
        _element = ElementPersistable.Create(
            specializationType: OperationModel.SpecializationType,
            specializationTypeId: OperationModel.SpecializationTypeId,
            name: getNameFunc(dataSource),
            parentId: null,
            externalReference: null);
    }

    public TOperation DataSource => _dataSource;
    
    public OperationElementBuilder<TOperation> AddParameter<TParam>(TParam dataSource, Func<TParam, ParameterBuilder> parameterBuilderFunc)
    {
        var builder = parameterBuilderFunc(dataSource);
        var result = builder.Build();
        _element.AddElement(result);
        _builderMetadataManager.SetTypeReferenceTypeId(result, builder.ParameterType);
        _paramTypes.Add(builder.ParameterType);
        return this;
    }

    public OperationElementBuilder<TOperation> AddParameters<TParam>(Func<TOperation, IEnumerable<TParam>> dataSource, Func<TParam, ParameterBuilder> parameterBuilderFunc)
    {
        foreach (var dataEntry in dataSource(_dataSource))
        {
            AddParameter(dataEntry, parameterBuilderFunc);
        }

        return this;
    }
    
    public OperationElementBuilder<TOperation> ReturnType(string? returnType, bool isNullable, bool isCollection)
    {
        _element.TypeReference = new TypeReferencePersistable
        {
            IsNullable = isNullable,
            IsCollection = isCollection
        };

        if (returnType is not null)
        {
            var genericType = _element.GenericTypes?.FirstOrDefault(p => p.Name == returnType);
            if (genericType is not null)
            {
                _element.TypeReference.TypeId = genericType.Id;
            }
            else
            {
                _builderMetadataManager.SetTypeReferenceTypeId(_element, returnType);
            }
        }

        return this;
    }
    
    public OperationElementBuilder<TOperation> AddGenericParameters(IReadOnlyList<string> genericParameters)
    {
        _element.GenericTypes = genericParameters.Select(type => new GenericType
        {
            Id = Guid.NewGuid().ToString(),
            Name = type
        }).ToList();
        return this;
    }

    protected override void PerformIntermediateBuildStep()
    {
        if (_element.Name is null)
        {
            throw new InvalidOperationException("Name is not set");
        }

        _element.ExternalReference = string.Join("+", new[] { _baseExternalReference, Name }.Concat(_paramTypes));
        foreach (var childElement in _element.ChildElements)
        {
            childElement.ExternalReference = $"{_element.ExternalReference}+{childElement.Name}";
        }
    }
}