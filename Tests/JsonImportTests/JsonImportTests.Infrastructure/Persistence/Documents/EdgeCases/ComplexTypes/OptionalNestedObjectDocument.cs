using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.EdgeCases.ComplexTypes;
using JsonImportTests.Domain.Repositories.Documents.EdgeCases.ComplexTypes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.EdgeCases.ComplexTypes
{
    internal class OptionalNestedObjectDocument : IOptionalNestedObjectDocument
    {
        public string RequiredField { get; set; } = default!;
        public object OptionalField { get; set; } = default!;
        public ConditionalDatumDocument ConditionalData { get; set; } = default!;
        IConditionalDatumDocument IOptionalNestedObjectDocument.ConditionalData => ConditionalData;

        public OptionalNestedObject ToEntity(OptionalNestedObject? entity = default)
        {
            entity ??= new OptionalNestedObject();

            entity.RequiredField = RequiredField ?? throw new Exception($"{nameof(entity.RequiredField)} is null");
            entity.OptionalField = OptionalField ?? throw new Exception($"{nameof(entity.OptionalField)} is null");
            entity.ConditionalData = ConditionalData.ToEntity() ?? throw new Exception($"{nameof(entity.ConditionalData)} is null");

            return entity;
        }

        public OptionalNestedObjectDocument PopulateFromEntity(OptionalNestedObject entity)
        {
            RequiredField = entity.RequiredField;
            OptionalField = entity.OptionalField;
            ConditionalData = ConditionalDatumDocument.FromEntity(entity.ConditionalData)!;

            return this;
        }

        public static OptionalNestedObjectDocument? FromEntity(OptionalNestedObject? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new OptionalNestedObjectDocument().PopulateFromEntity(entity);
        }
    }
}