using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Transactions
{
    public interface ISettlementDetailDocument
    {
        string SettlementCurrency { get; }
        decimal ExchangeRate { get; }
        decimal SettlementAmount { get; }
        string SettlementMethod { get; }
        string CustodianReference { get; }
        string ClearingHouse { get; }
    }
}