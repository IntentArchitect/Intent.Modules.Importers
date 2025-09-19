using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Portfolios
{
    public interface IRestrictionDocument
    {
        decimal MaxPositionSize { get; }
        IReadOnlyList<string> ProhibitedAssets { get; }
        decimal MinCashPercentage { get; }
        decimal MaxSectorConcentration { get; }
    }
}