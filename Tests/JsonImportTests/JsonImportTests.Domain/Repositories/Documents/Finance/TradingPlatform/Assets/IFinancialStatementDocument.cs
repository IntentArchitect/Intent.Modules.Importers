using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Assets
{
    public interface IFinancialStatementDocument
    {
        DateTime LastReportingDate { get; }
        string ReportingFrequency { get; }
        decimal Revenue { get; }
        decimal NetIncome { get; }
        decimal TotalAssets { get; }
        decimal TotalLiabilities { get; }
        decimal ShareholdersEquity { get; }
        decimal FreeCashFlow { get; }
        decimal OperatingCashFlow { get; }
    }
}