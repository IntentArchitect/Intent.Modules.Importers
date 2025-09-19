using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.Academic;
using JsonImportTests.Domain.Repositories.Documents.Education.University.Academic;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.Academic
{
    internal class LearningOutcomeDocument : ILearningOutcomeDocument
    {
        public string Id { get; set; } = default!;
        public Guid OutcomeId { get; set; }
        public string Description { get; set; } = default!;
        public string BloomLevel { get; set; } = default!;
        public List<string> AssessmentMethods { get; set; } = default!;
        IReadOnlyList<string> ILearningOutcomeDocument.AssessmentMethods => AssessmentMethods;

        public LearningOutcome ToEntity(LearningOutcome? entity = default)
        {
            entity ??= new LearningOutcome();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.OutcomeId = OutcomeId;
            entity.Description = Description ?? throw new Exception($"{nameof(entity.Description)} is null");
            entity.BloomLevel = BloomLevel ?? throw new Exception($"{nameof(entity.BloomLevel)} is null");
            entity.AssessmentMethods = AssessmentMethods ?? throw new Exception($"{nameof(entity.AssessmentMethods)} is null");

            return entity;
        }

        public LearningOutcomeDocument PopulateFromEntity(LearningOutcome entity)
        {
            Id = entity.Id;
            OutcomeId = entity.OutcomeId;
            Description = entity.Description;
            BloomLevel = entity.BloomLevel;
            AssessmentMethods = entity.AssessmentMethods.ToList();

            return this;
        }

        public static LearningOutcomeDocument? FromEntity(LearningOutcome? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new LearningOutcomeDocument().PopulateFromEntity(entity);
        }
    }
}