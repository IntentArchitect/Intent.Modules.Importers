using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.Students;
using JsonImportTests.Domain.Repositories.Documents.Education.University.Students;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.Students
{
    internal class StudentFinancialInfoDocument : IStudentFinancialInfoDocument
    {
        public string TuitionStatus { get; set; } = default!;
        public bool FinancialAidEligible { get; set; }
        public decimal OutstandingBalance { get; set; }
        public List<StudentLoanDocument> StudentLoans { get; set; } = default!;
        IReadOnlyList<IStudentLoanDocument> IStudentFinancialInfoDocument.StudentLoans => StudentLoans;
        public List<ScholarshipsAndGrantDocument> ScholarshipsAndGrants { get; set; } = default!;
        IReadOnlyList<IScholarshipsAndGrantDocument> IStudentFinancialInfoDocument.ScholarshipsAndGrants => ScholarshipsAndGrants;
        public PaymentPlanDocument PaymentPlan { get; set; } = default!;
        IPaymentPlanDocument IStudentFinancialInfoDocument.PaymentPlan => PaymentPlan;

        public StudentFinancialInfo ToEntity(StudentFinancialInfo? entity = default)
        {
            entity ??= new StudentFinancialInfo();

            entity.TuitionStatus = TuitionStatus ?? throw new Exception($"{nameof(entity.TuitionStatus)} is null");
            entity.FinancialAidEligible = FinancialAidEligible;
            entity.OutstandingBalance = OutstandingBalance;
            entity.StudentLoans = StudentLoans.Select(x => x.ToEntity()).ToList();
            entity.ScholarshipsAndGrants = ScholarshipsAndGrants.Select(x => x.ToEntity()).ToList();
            entity.PaymentPlan = PaymentPlan.ToEntity() ?? throw new Exception($"{nameof(entity.PaymentPlan)} is null");

            return entity;
        }

        public StudentFinancialInfoDocument PopulateFromEntity(StudentFinancialInfo entity)
        {
            TuitionStatus = entity.TuitionStatus;
            FinancialAidEligible = entity.FinancialAidEligible;
            OutstandingBalance = entity.OutstandingBalance;
            StudentLoans = entity.StudentLoans.Select(x => StudentLoanDocument.FromEntity(x)!).ToList();
            ScholarshipsAndGrants = entity.ScholarshipsAndGrants.Select(x => ScholarshipsAndGrantDocument.FromEntity(x)!).ToList();
            PaymentPlan = PaymentPlanDocument.FromEntity(entity.PaymentPlan)!;

            return this;
        }

        public static StudentFinancialInfoDocument? FromEntity(StudentFinancialInfo? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new StudentFinancialInfoDocument().PopulateFromEntity(entity);
        }
    }
}