using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Education.University.EducationEnrollment
{
    public class AuditStatus
    {
        public AuditStatus()
        {
            AuditApprovalDate = null!;
        }

        public bool IsAudit { get; set; }

        public object AuditApprovalDate { get; set; }

        public decimal AuditFee { get; set; }
    }
}