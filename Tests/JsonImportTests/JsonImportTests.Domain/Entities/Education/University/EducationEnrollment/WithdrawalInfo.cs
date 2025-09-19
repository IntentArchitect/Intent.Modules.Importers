using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Education.University.EducationEnrollment
{
    public class WithdrawalInfo
    {
        public WithdrawalInfo()
        {
            WithdrawalDate = null!;
            WithdrawalReason = null!;
            WithdrawalType = null!;
            RefundPercentage = null!;
            WithdrawnBy = null!;
            AcademicImpact = null!;
        }

        public object WithdrawalDate { get; set; }

        public object WithdrawalReason { get; set; }

        public object WithdrawalType { get; set; }

        public object RefundPercentage { get; set; }

        public object WithdrawnBy { get; set; }

        public object AcademicImpact { get; set; }
    }
}