using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.EdgeCases.ComplexTypes;
using JsonImportTests.Domain.Repositories.Documents.EdgeCases.ComplexTypes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.EdgeCases.ComplexTypes
{
    internal class TimestampDocument : ITimestampDocument
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public object LastAccessedAt { get; set; } = default!;
        public DateTime ScheduledAt { get; set; }
        public object CompletedAt { get; set; } = default!;
        public object DeletedAt { get; set; } = default!;

        public Timestamp ToEntity(Timestamp? entity = default)
        {
            entity ??= new Timestamp();

            entity.CreatedAt = CreatedAt;
            entity.UpdatedAt = UpdatedAt;
            entity.LastAccessedAt = LastAccessedAt ?? throw new Exception($"{nameof(entity.LastAccessedAt)} is null");
            entity.ScheduledAt = ScheduledAt;
            entity.CompletedAt = CompletedAt ?? throw new Exception($"{nameof(entity.CompletedAt)} is null");
            entity.DeletedAt = DeletedAt ?? throw new Exception($"{nameof(entity.DeletedAt)} is null");

            return entity;
        }

        public TimestampDocument PopulateFromEntity(Timestamp entity)
        {
            CreatedAt = entity.CreatedAt;
            UpdatedAt = entity.UpdatedAt;
            LastAccessedAt = entity.LastAccessedAt;
            ScheduledAt = entity.ScheduledAt;
            CompletedAt = entity.CompletedAt;
            DeletedAt = entity.DeletedAt;

            return this;
        }

        public static TimestampDocument? FromEntity(Timestamp? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new TimestampDocument().PopulateFromEntity(entity);
        }
    }
}