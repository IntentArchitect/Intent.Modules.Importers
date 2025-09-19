using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Transactions
{
    public interface IAmendmentDocument
    {
        Guid AmendmentId { get; }
        string AmendmentType { get; }
        string AmendmentReason { get; }
        string OriginalValue { get; }
        string NewValue { get; }
        string AmendedBy { get; }
        DateTime AmendedAt { get; }
    }
}