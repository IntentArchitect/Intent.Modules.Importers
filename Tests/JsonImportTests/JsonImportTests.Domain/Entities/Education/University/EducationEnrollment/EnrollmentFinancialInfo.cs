using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Education.University.EducationEnrollment
{
    public class EnrollmentFinancialInfo
    {
        public EnrollmentFinancialInfo()
        {
            PaymentStatus = null!;
        }

        public decimal TuitionCharged { get; set; }

        public decimal FeesCharged { get; set; }

        public decimal FinancialAidApplied { get; set; }

        public decimal OutstandingBalance { get; set; }

        public string PaymentStatus { get; set; }

        public bool RefundEligible { get; set; }

        public decimal RefundAmount { get; set; }
    }
}