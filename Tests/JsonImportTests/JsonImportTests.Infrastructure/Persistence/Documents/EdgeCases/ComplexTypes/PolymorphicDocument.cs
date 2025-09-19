using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.EdgeCases.ComplexTypes;
using JsonImportTests.Domain.Repositories.Documents.EdgeCases.ComplexTypes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.EdgeCases.ComplexTypes
{
    internal class PolymorphicDocument : IPolymorphicDocument
    {
        public string Id { get; set; } = default!;
        public string Type { get; set; } = default!;
        public decimal Radius { get; set; }
        public string Color { get; set; } = default!;

        public Polymorphic ToEntity(Polymorphic? entity = default)
        {
            entity ??= new Polymorphic();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Type = Type ?? throw new Exception($"{nameof(entity.Type)} is null");
            entity.Radius = Radius;
            entity.Color = Color ?? throw new Exception($"{nameof(entity.Color)} is null");

            return entity;
        }

        public PolymorphicDocument PopulateFromEntity(Polymorphic entity)
        {
            Id = entity.Id;
            Type = entity.Type;
            Radius = entity.Radius;
            Color = entity.Color;

            return this;
        }

        public static PolymorphicDocument? FromEntity(Polymorphic? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new PolymorphicDocument().PopulateFromEntity(entity);
        }
    }
}