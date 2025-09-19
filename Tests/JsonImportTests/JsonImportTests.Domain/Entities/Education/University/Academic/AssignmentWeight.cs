using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Education.University.Academic
{
    public class AssignmentWeight
    {
        private string? _id;

        public AssignmentWeight()
        {
            Id = null!;
            Category = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public string Category { get; set; }

        public decimal Weight { get; set; }
    }
}