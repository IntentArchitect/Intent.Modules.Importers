using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Transactions
{
    public interface IFeeDocument
    {
        decimal Commission { get; }
        decimal RegulatoryFees { get; }
        decimal ExchangeFees { get; }
        decimal ClearingFees { get; }
        decimal OtherFees { get; }
        decimal TotalFees { get; }
        string Currency { get; }
    }
}