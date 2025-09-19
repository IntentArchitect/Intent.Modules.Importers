using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Assets
{
    public interface ITradingInfoDocument
    {
        string PrimaryExchange { get; }
        IReadOnlyList<string> SecondaryExchanges { get; }
        decimal TickSize { get; }
        decimal LotSize { get; }
        bool IsShortable { get; }
        bool IsMarginable { get; }
        bool DividendEligible { get; }
        ITradingHourDocument TradingHours { get; }
    }
}