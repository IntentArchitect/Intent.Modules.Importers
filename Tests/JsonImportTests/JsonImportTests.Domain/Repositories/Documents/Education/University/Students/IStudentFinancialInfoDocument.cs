using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Education.University.Students
{
    public interface IStudentFinancialInfoDocument
    {
        string TuitionStatus { get; }
        bool FinancialAidEligible { get; }
        decimal OutstandingBalance { get; }
        IReadOnlyList<IStudentLoanDocument> StudentLoans { get; }
        IReadOnlyList<IScholarshipsAndGrantDocument> ScholarshipsAndGrants { get; }
        IPaymentPlanDocument PaymentPlan { get; }
    }
}