using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.EdgeCases.ComplexTypes;
using JsonImportTests.Domain.Repositories.Documents.EdgeCases.ComplexTypes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.EdgeCases.ComplexTypes
{
    internal class Key5Document : IKey5Document
    {
        public string Nested { get; set; } = default!;
        public List<string> With { get; set; } = default!;
        IReadOnlyList<string> IKey5Document.With => With;

        public Key5 ToEntity(Key5? entity = default)
        {
            entity ??= new Key5();

            entity.Nested = Nested ?? throw new Exception($"{nameof(entity.Nested)} is null");
            entity.With = With ?? throw new Exception($"{nameof(entity.With)} is null");

            return entity;
        }

        public Key5Document PopulateFromEntity(Key5 entity)
        {
            Nested = entity.Nested;
            With = entity.With.ToList();

            return this;
        }

        public static Key5Document? FromEntity(Key5? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new Key5Document().PopulateFromEntity(entity);
        }
    }
}