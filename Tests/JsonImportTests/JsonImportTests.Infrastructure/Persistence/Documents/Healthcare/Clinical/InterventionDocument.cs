using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Healthcare.Clinical;
using JsonImportTests.Domain.Repositories.Documents.Healthcare.Clinical;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Healthcare.Clinical
{
    internal class InterventionDocument : IInterventionDocument
    {
        public string Id { get; set; } = default!;
        public string Type { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string Frequency { get; set; } = default!;
        public string Duration { get; set; } = default!;

        public Intervention ToEntity(Intervention? entity = default)
        {
            entity ??= new Intervention();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Type = Type ?? throw new Exception($"{nameof(entity.Type)} is null");
            entity.Description = Description ?? throw new Exception($"{nameof(entity.Description)} is null");
            entity.Frequency = Frequency ?? throw new Exception($"{nameof(entity.Frequency)} is null");
            entity.Duration = Duration ?? throw new Exception($"{nameof(entity.Duration)} is null");

            return entity;
        }

        public InterventionDocument PopulateFromEntity(Intervention entity)
        {
            Id = entity.Id;
            Type = entity.Type;
            Description = entity.Description;
            Frequency = entity.Frequency;
            Duration = entity.Duration;

            return this;
        }

        public static InterventionDocument? FromEntity(Intervention? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new InterventionDocument().PopulateFromEntity(entity);
        }
    }
}