using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Education.University.EducationEnrollment
{
    public class GradingInfo
    {
        public GradingInfo()
        {
            CurrentGrade = null!;
            IncompleteReason = null!;
        }

        public string CurrentGrade { get; set; }

        public decimal CurrentPercentage { get; set; }

        public decimal GradePoints { get; set; }

        public decimal QualityPoints { get; set; }

        public bool IsPassFail { get; set; }

        public object IncompleteReason { get; set; }

        public ICollection<GradeChangeHistory> GradeChangeHistory { get; set; } = [];
    }
}