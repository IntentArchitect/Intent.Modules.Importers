using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Education.University.Students
{
    public class StudentLoan
    {
        private string? _id;

        public StudentLoan()
        {
            Id = null!;
            LoanType = null!;
            Status = null!;
            Servicer = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public Guid LoanId { get; set; }

        public string LoanType { get; set; }

        public decimal Amount { get; set; }

        public decimal InterestRate { get; set; }

        public string Status { get; set; }

        public string Servicer { get; set; }
    }
}