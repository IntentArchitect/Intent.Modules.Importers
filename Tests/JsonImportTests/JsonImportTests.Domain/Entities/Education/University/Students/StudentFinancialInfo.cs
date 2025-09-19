using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Education.University.Students
{
    public class StudentFinancialInfo
    {
        public StudentFinancialInfo()
        {
            TuitionStatus = null!;
            PaymentPlan = null!;
        }

        public string TuitionStatus { get; set; }

        public bool FinancialAidEligible { get; set; }

        public decimal OutstandingBalance { get; set; }

        public ICollection<StudentLoan> StudentLoans { get; set; } = [];

        public ICollection<ScholarshipsAndGrant> ScholarshipsAndGrants { get; set; } = [];

        public PaymentPlan PaymentPlan { get; set; }
    }
}