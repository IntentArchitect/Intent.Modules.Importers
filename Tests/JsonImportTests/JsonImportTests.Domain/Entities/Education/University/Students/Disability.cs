using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Education.University.Students
{
    public class Disability
    {
        private string? _id;

        public Disability()
        {
            Id = null!;
            DisabilityType = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public string DisabilityType { get; set; }

        public IList<string> AccommodationsNeeded { get; set; } = [];

        public DateTime ApprovedDate { get; set; }
    }
}