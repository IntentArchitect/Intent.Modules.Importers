using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Transactions
{
    public interface IExecutionQualityDocument
    {
        decimal PriceImprovement { get; }
        decimal SpreadCaptured { get; }
        decimal TimeToExecution { get; }
    }
}