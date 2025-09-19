using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Portfolios
{
    public interface IPortfolioDocument
    {
        string Id { get; }
        Guid AccountId { get; }
        string PortfolioName { get; }
        string PortfolioType { get; }
        string InvestmentObjective { get; }
        string RiskTolerance { get; }
        DateTime CreatedDate { get; }
        DateTime LastRebalanceDate { get; }
        DateTime LastValuationDate { get; }
        bool IsActive { get; }
        IPortfolioRiskMetricDocument RiskMetrics { get; }
        IRestrictionDocument Restrictions { get; }
        IPerformanceDocument Performance { get; }
        IOwnerDocument Owner { get; }
        IReadOnlyList<IHoldingDocument> Holdings { get; }
        IAssetAllocationDocument AssetAllocation { get; }
        IAccountDetailDocument AccountDetails { get; }
    }
}