using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.EdgeCases.ComplexTypes;
using JsonImportTests.Domain.Repositories.Documents.EdgeCases.ComplexTypes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.EdgeCases.ComplexTypes
{
    internal class MixedTypeArrayDocument : IMixedTypeArrayDocument
    {
        public string Id { get; set; } = default!;
        public string Type { get; set; } = default!;
        public string Value { get; set; } = default!;
        public bool IsValid { get; set; }

        public MixedTypeArray ToEntity(MixedTypeArray? entity = default)
        {
            entity ??= new MixedTypeArray();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Type = Type ?? throw new Exception($"{nameof(entity.Type)} is null");
            entity.Value = Value ?? throw new Exception($"{nameof(entity.Value)} is null");
            entity.IsValid = IsValid;

            return entity;
        }

        public MixedTypeArrayDocument PopulateFromEntity(MixedTypeArray entity)
        {
            Id = entity.Id;
            Type = entity.Type;
            Value = entity.Value;
            IsValid = entity.IsValid;

            return this;
        }

        public static MixedTypeArrayDocument? FromEntity(MixedTypeArray? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new MixedTypeArrayDocument().PopulateFromEntity(entity);
        }
    }
}