using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Transactions
{
    public interface ITransactionPricingDocument
    {
        decimal OrderPrice { get; }
        decimal ExecutedPrice { get; }
        decimal GrossAmount { get; }
        decimal NetAmount { get; }
        decimal MarketValue { get; }
        decimal AccruedInterest { get; }
    }
}