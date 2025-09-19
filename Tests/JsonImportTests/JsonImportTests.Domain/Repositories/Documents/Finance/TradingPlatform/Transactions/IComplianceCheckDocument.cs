using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Transactions
{
    public interface IComplianceCheckDocument
    {
        string Id { get; }
        string CheckType { get; }
        string Status { get; }
        DateTime CheckedAt { get; }
    }
}