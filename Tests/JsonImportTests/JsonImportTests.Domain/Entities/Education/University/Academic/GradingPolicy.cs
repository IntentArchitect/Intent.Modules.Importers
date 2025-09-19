using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Education.University.Academic
{
    public class GradingPolicy
    {
        public ICollection<GradingScale> GradingScale { get; set; } = [];

        public ICollection<AssignmentWeight> AssignmentWeights { get; set; } = [];
    }
}