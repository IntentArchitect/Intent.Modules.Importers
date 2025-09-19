using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Education.University.EducationEnrollment
{
    public class LateEnrollment
    {
        public LateEnrollment()
        {
            ApprovedBy = null!;
        }

        public bool IsLateEnrollment { get; set; }

        public decimal LateEnrollmentFee { get; set; }

        public bool ApprovalRequired { get; set; }

        public object ApprovedBy { get; set; }
    }
}