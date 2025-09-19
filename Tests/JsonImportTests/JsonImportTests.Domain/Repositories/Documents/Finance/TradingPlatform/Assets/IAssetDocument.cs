using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Assets
{
    public interface IAssetDocument
    {
        string Id { get; }
        string Symbol { get; }
        string Name { get; }
        string ISIN { get; }
        string CUSIP { get; }
        string AssetType { get; }
        string AssetClass { get; }
        string Sector { get; }
        string Industry { get; }
        string Exchange { get; }
        string Currency { get; }
        string Country { get; }
        ITradingInfoDocument TradingInfo { get; }
        IAssetRiskMetricDocument RiskMetrics { get; }
        IPricingDatumDocument PricingData { get; }
        IAssetMetadataDocument Metadata { get; }
        IFundamentalDocument Fundamentals { get; }
        IFinancialStatementDocument FinancialStatements { get; }
        ICompanyInfoDocument CompanyInfo { get; }
        IAnalystDocument Analysts { get; }
    }
}