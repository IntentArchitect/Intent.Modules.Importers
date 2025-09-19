using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.EdgeCases.ComplexTypes;
using JsonImportTests.Domain.Repositories.Documents.EdgeCases.ComplexTypes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.EdgeCases.ComplexTypes
{
    internal class PreferenceDocument : IPreferenceDocument
    {
        public string Id { get; set; } = default!;
        public string Category { get; set; } = default!;
        public string Key { get; set; } = default!;
        public string Value { get; set; } = default!;
        public string Type { get; set; } = default!;

        public Preference ToEntity(Preference? entity = default)
        {
            entity ??= new Preference();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Category = Category ?? throw new Exception($"{nameof(entity.Category)} is null");
            entity.Key = Key ?? throw new Exception($"{nameof(entity.Key)} is null");
            entity.Value = Value ?? throw new Exception($"{nameof(entity.Value)} is null");
            entity.Type = Type ?? throw new Exception($"{nameof(entity.Type)} is null");

            return entity;
        }

        public PreferenceDocument PopulateFromEntity(Preference entity)
        {
            Id = entity.Id;
            Category = entity.Category;
            Key = entity.Key;
            Value = entity.Value;
            Type = entity.Type;

            return this;
        }

        public static PreferenceDocument? FromEntity(Preference? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new PreferenceDocument().PopulateFromEntity(entity);
        }
    }
}