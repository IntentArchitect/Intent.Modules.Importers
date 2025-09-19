using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Education.University.EducationEnrollment
{
    public interface IEnrollmentFinancialInfoDocument
    {
        decimal TuitionCharged { get; }
        decimal FeesCharged { get; }
        decimal FinancialAidApplied { get; }
        decimal OutstandingBalance { get; }
        string PaymentStatus { get; }
        bool RefundEligible { get; }
        decimal RefundAmount { get; }
    }
}