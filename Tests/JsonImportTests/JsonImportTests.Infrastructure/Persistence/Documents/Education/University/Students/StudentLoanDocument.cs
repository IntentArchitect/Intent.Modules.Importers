using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.Students;
using JsonImportTests.Domain.Repositories.Documents.Education.University.Students;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.Students
{
    internal class StudentLoanDocument : IStudentLoanDocument
    {
        public string Id { get; set; } = default!;
        public Guid LoanId { get; set; }
        public string LoanType { get; set; } = default!;
        public decimal Amount { get; set; }
        public decimal InterestRate { get; set; }
        public string Status { get; set; } = default!;
        public string Servicer { get; set; } = default!;

        public StudentLoan ToEntity(StudentLoan? entity = default)
        {
            entity ??= new StudentLoan();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.LoanId = LoanId;
            entity.LoanType = LoanType ?? throw new Exception($"{nameof(entity.LoanType)} is null");
            entity.Amount = Amount;
            entity.InterestRate = InterestRate;
            entity.Status = Status ?? throw new Exception($"{nameof(entity.Status)} is null");
            entity.Servicer = Servicer ?? throw new Exception($"{nameof(entity.Servicer)} is null");

            return entity;
        }

        public StudentLoanDocument PopulateFromEntity(StudentLoan entity)
        {
            Id = entity.Id;
            LoanId = entity.LoanId;
            LoanType = entity.LoanType;
            Amount = entity.Amount;
            InterestRate = entity.InterestRate;
            Status = entity.Status;
            Servicer = entity.Servicer;

            return this;
        }

        public static StudentLoanDocument? FromEntity(StudentLoan? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new StudentLoanDocument().PopulateFromEntity(entity);
        }
    }
}