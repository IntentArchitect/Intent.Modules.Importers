using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Education.University.Students
{
    public interface IStudentLoanDocument
    {
        string Id { get; }
        Guid LoanId { get; }
        string LoanType { get; }
        decimal Amount { get; }
        decimal InterestRate { get; }
        string Status { get; }
        string Servicer { get; }
    }
}