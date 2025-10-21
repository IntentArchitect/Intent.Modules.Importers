using Intent.IArchitect.Agent.Persistence.Model;
using Intent.Modules.Common.Types.Api;

namespace Intent.MetadataSynchronizer.CSharp.Importer.Builders;

public class EnumElementBuilder : ElementBuilderBase
{
    public EnumElementBuilder(string externalReference, string name, BuilderMetadataManager builderMetadataManager)
    {
        _element = ElementPersistable.Create(
            specializationType: EnumModel.SpecializationType,
            specializationTypeId: EnumModel.SpecializationTypeId,
            name: name,
            parentId: null,
            externalReference: externalReference);
    }

    public EnumElementBuilder SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            ArgumentException.ThrowIfNullOrEmpty(nameof(name));
            return this;
        }

        _element.Name = name;
        return this;
    }

    public EnumElementBuilder SetParentId(string? parentId)
    {
        if (string.IsNullOrWhiteSpace(parentId))
        {
            ArgumentException.ThrowIfNullOrEmpty(nameof(parentId));
            return this;
        }

        _element.ParentFolderId = parentId;
        return this;
    }

    public EnumElementBuilder AddLiteral(string literalName, string? literalValue)
    {
        var literalElement = ElementPersistable.Create(
            specializationType: EnumLiteralModel.SpecializationType,
            specializationTypeId: EnumLiteralModel.SpecializationTypeId,
            name: literalName,
            parentId: _element.Id,
            externalReference: $"{_element.ExternalReference}+{literalName}");
        literalElement.Value = literalValue;
        _element.AddElement(literalElement);
        return this;
    }
}