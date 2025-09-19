using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Education.University.EducationEnrollment
{
    public class SpecialCircumstance
    {
        public SpecialCircumstance()
        {
            LateEnrollment = null!;
            IncompleteStatus = null!;
            AuditStatus = null!;
        }

        public LateEnrollment LateEnrollment { get; set; }

        public IncompleteStatus IncompleteStatus { get; set; }

        public AuditStatus AuditStatus { get; set; }
    }
}