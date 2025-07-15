using Intent.IArchitect.Agent.Persistence.Model;
using Intent.Modelers.Services.Api;

namespace Intent.MetadataSynchronizer.CSharp.CLI.Builders;

public class DtoElementBuilder : ElementBuilderBase
{
    private readonly BuilderMetadataManager _builderMetadataManager;

    public DtoElementBuilder(string externalReference, string name, BuilderMetadataManager builderMetadataManager)
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
            specializationType: DTOModel.SpecializationType,
            specializationTypeId: DTOModel.SpecializationTypeId,
            name: name,
            parentId: null,
            externalReference: externalReference);
    }
    
    public DtoElementBuilder SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            ArgumentException.ThrowIfNullOrEmpty(nameof(name));
            return this;
        }

        _element.Name = name;
        return this;
    }

    public DtoElementBuilder SetParentId(string? parentId)
    {
        if (string.IsNullOrWhiteSpace(parentId))
        {
            ArgumentException.ThrowIfNullOrEmpty(nameof(parentId));
            return this;
        }

        _element.ParentFolderId = parentId;
        return this;
    }
    
    public DtoElementBuilder AddField<TField>(TField dataSource, Func<TField, DtoFieldBuilder> attributeBuilderFunc)
    {
        var builder = attributeBuilderFunc(dataSource);
        var result = builder.Build();
        _builderMetadataManager.SetTypeReferenceTypeId(result, builder.FieldType);
        _element.AddElement(result);
        return this;
    }

    public DtoElementBuilder AddFields<TField>(IEnumerable<TField> dataSource, Func<TField, DtoFieldBuilder> attributeBuilderFunc)
    {
        foreach (var dataEntry in dataSource)
        {
            AddField(dataEntry, attributeBuilderFunc);
        }

        return this;
    }
    
    protected override void PerformIntermediateBuildStep()
    {
        foreach (var childElement in _element.ChildElements.Where(p => p.SpecializationTypeId == DTOFieldModel.SpecializationTypeId))
        {
            childElement.ExternalReference = $"{_element.ExternalReference}+{childElement.Name}";
        }
    }
}