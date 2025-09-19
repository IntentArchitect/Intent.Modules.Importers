using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Transactions
{
    public interface IAllocationDocument
    {
        string Id { get; }
        Guid AccountId { get; }
        decimal AllocationPercentage { get; }
        decimal AllocatedQuantity { get; }
        decimal AllocatedAmount { get; }
    }
}