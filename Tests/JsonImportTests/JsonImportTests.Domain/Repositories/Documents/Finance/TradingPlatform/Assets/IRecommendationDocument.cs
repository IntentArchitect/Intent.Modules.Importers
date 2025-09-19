using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Assets
{
    public interface IRecommendationDocument
    {
        decimal StrongBuy { get; }
        decimal Buy { get; }
        decimal Hold { get; }
        decimal Sell { get; }
        decimal StrongSell { get; }
    }
}