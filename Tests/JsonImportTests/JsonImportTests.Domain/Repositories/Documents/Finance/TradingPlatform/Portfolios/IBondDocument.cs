using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Portfolios
{
    public interface IBondDocument
    {
        decimal Percentage { get; }
        decimal Value { get; }
        IReadOnlyList<ITypeDocument> Types { get; }
    }
}