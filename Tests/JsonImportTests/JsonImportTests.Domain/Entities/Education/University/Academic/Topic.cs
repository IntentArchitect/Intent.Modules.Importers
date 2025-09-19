using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Education.University.Academic
{
    public class Topic
    {
        private string? _id;

        public Topic()
        {
            Id = null!;
            Title = null!;
            Description = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public string Title { get; set; }

        public string Description { get; set; }

        public decimal EstimatedHours { get; set; }
    }
}