using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Education.University.Academic
{
    public class GradingScale
    {
        private string? _id;

        public GradingScale()
        {
            Id = null!;
            Grade = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public string Grade { get; set; }

        public decimal MinPercentage { get; set; }

        public decimal MaxPercentage { get; set; }

        public decimal GradePoints { get; set; }
    }
}