using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Healthcare.Clinical;
using JsonImportTests.Domain.Repositories.Documents.Healthcare.Clinical;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Healthcare.Clinical
{
    internal class SymptomDocument : ISymptomDocument
    {
        public string Id { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string Severity { get; set; } = default!;
        public string Duration { get; set; } = default!;
        public DateTime OnsetDate { get; set; }

        public Symptom ToEntity(Symptom? entity = default)
        {
            entity ??= new Symptom();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Description = Description ?? throw new Exception($"{nameof(entity.Description)} is null");
            entity.Severity = Severity ?? throw new Exception($"{nameof(entity.Severity)} is null");
            entity.Duration = Duration ?? throw new Exception($"{nameof(entity.Duration)} is null");
            entity.OnsetDate = OnsetDate;

            return entity;
        }

        public SymptomDocument PopulateFromEntity(Symptom entity)
        {
            Id = entity.Id;
            Description = entity.Description;
            Severity = entity.Severity;
            Duration = entity.Duration;
            OnsetDate = entity.OnsetDate;

            return this;
        }

        public static SymptomDocument? FromEntity(Symptom? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new SymptomDocument().PopulateFromEntity(entity);
        }
    }
}