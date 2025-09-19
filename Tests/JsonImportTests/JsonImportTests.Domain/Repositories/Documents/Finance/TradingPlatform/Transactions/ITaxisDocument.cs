using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Transactions
{
    public interface ITaxisDocument
    {
        decimal TransactionTax { get; }
        decimal StampDuty { get; }
        decimal WithholdingTax { get; }
        decimal TotalTaxes { get; }
        string TaxCurrency { get; }
    }
}