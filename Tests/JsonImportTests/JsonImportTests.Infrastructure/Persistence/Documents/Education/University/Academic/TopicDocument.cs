using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.Academic;
using JsonImportTests.Domain.Repositories.Documents.Education.University.Academic;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.Academic
{
    internal class TopicDocument : ITopicDocument
    {
        public string Id { get; set; } = default!;
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public decimal EstimatedHours { get; set; }

        public Topic ToEntity(Topic? entity = default)
        {
            entity ??= new Topic();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Title = Title ?? throw new Exception($"{nameof(entity.Title)} is null");
            entity.Description = Description ?? throw new Exception($"{nameof(entity.Description)} is null");
            entity.EstimatedHours = EstimatedHours;

            return entity;
        }

        public TopicDocument PopulateFromEntity(Topic entity)
        {
            Id = entity.Id;
            Title = entity.Title;
            Description = entity.Description;
            EstimatedHours = entity.EstimatedHours;

            return this;
        }

        public static TopicDocument? FromEntity(Topic? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new TopicDocument().PopulateFromEntity(entity);
        }
    }
}