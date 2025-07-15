using System.Xml.Serialization;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.IArchitect.Agent.Persistence.Model.Mappings;

namespace Intent.MetadataSynchronizer.CSharp.CLI.Builders;

public class ElementMappingBuilder
{
    private ElementToElementMappingPersistable _mapping;

    public ElementMappingBuilder()
    {
        _mapping = new ElementToElementMappingPersistable();
    }

    public ElementMappingBuilder SetType(string type)
    {
        _mapping.Type = type;
        return this;
    }

    public ElementMappingBuilder SetTypeId(string typeId)
    {
        _mapping.TypeId = typeId;
        return this;
    }

    public ElementMappingBuilder SetSource(string applicationId, string designerId, string elementId, string location)
    {
        _mapping.Source = new ElementSolutionIdentifierPersistable
        {
            ApplicationId = applicationId,
            DesignerId = designerId,
            ElementId = elementId,
            Location = location
        };

        return this;
    }

    public ElementMappingBuilder SetTarget(string applicationId, string designerId, string elementId, string location)
    {
        _mapping.Target = new ElementSolutionIdentifierPersistable
        {
            ApplicationId = applicationId,
            DesignerId = designerId,
            ElementId = elementId,
            Location = location
        };

        return this;
    }

    public ElementMappingBuilder AddMappedEnd(Action<MappedEndBuilder> configureMappedEnd)
    {
        var mappedEnd = new MappedEndBuilder();
        configureMappedEnd(mappedEnd);
        _mapping.MappedEnds.Add(mappedEnd.Build());

        return this;
    }

    public ElementToElementMappingPersistable Build()
    {
        return _mapping;
    }
}

public class MappedEndBuilder
{
    private ElementToElementMappedEndPersistable _mappedEnd = new ElementToElementMappedEndPersistable();

    public MappedEndBuilder SetExpression(string expression)
    {
        _mappedEnd.MappingExpression = expression;
        return this;
    }

    public MappedEndBuilder AddSource(string identifier, string mappingType, string mappingTypeId, Action<MappedEndSourceBuilder> configurePath)
    {
        var source = new ElementToElementMappedEndSourcePersistable
        {
            ExpressionIdentifier = identifier,
            MappingType = mappingType,
            MappingTypeId = mappingTypeId
        };

        var sourceBuilder = new MappedEndSourceBuilder();
        configurePath(sourceBuilder);
        source.Path = sourceBuilder.Build();

        _mappedEnd.Sources.Add(source);
        return this;
    }

    public MappedEndBuilder AddTargetPath(Action<MappedPathBuilder> configurePath)
    {
        var pathBuilder = new MappedPathBuilder();
        configurePath(pathBuilder);
        _mappedEnd.TargetPath = pathBuilder.Build();

        return this;
    }

    public ElementToElementMappedEndPersistable Build()
    {
        return _mappedEnd;
    }
}

public class MappedEndSourceBuilder
{
    private List<MappedPathTargetPersistable> _pathTargets = new List<MappedPathTargetPersistable>();

    public MappedEndSourceBuilder AddTarget(string id, string name, ElementType type, string specialization)
    {
        _pathTargets.Add(new MappedPathTargetPersistable
        {
            Id = id,
            Name = name,
            Type = type.ToString(),
            Specialization = specialization
        });
        return this;
    }

    public List<MappedPathTargetPersistable> Build()
    {
        return _pathTargets;
    }
}

public class MappedPathBuilder
{
    private List<MappedPathTargetPersistable> _pathTargets = new List<MappedPathTargetPersistable>();

    public MappedPathBuilder AddTarget(string id, string name, ElementType type, string specialization)
    {
        _pathTargets.Add(new MappedPathTargetPersistable
        {
            Id = id,
            Name = name,
            Type = type.ToString(),
            Specialization = specialization
        });
        return this;
    }

    public List<MappedPathTargetPersistable> Build()
    {
        return _pathTargets;
    }
}