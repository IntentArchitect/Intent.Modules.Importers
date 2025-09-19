using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Education.University.Academic
{
    public class LearningOutcome
    {
        private string? _id;

        public LearningOutcome()
        {
            Id = null!;
            Description = null!;
            BloomLevel = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public Guid OutcomeId { get; set; }

        public string Description { get; set; }

        public string BloomLevel { get; set; }

        public IList<string> AssessmentMethods { get; set; } = [];
    }
}