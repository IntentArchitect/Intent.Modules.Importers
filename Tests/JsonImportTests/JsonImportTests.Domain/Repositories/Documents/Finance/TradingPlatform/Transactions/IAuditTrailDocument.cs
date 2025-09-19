using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Transactions
{
    public interface IAuditTrailDocument
    {
        string CreatedBy { get; }
        DateTime CreatedAt { get; }
        string LastModifiedBy { get; }
        DateTime LastModifiedAt { get; }
        decimal Version { get; }
        string Source { get; }
    }
}