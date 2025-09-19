using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Finance.TradingPlatform.Portfolios
{
    public class Portfolio
    {
        private Guid? _id;

        public Portfolio()
        {
            PortfolioName = null!;
            PortfolioType = null!;
            InvestmentObjective = null!;
            RiskTolerance = null!;
            RiskMetrics = null!;
            Restrictions = null!;
            Performance = null!;
            Owner = null!;
            AssetAllocation = null!;
            AccountDetails = null!;
        }

        public Guid Id
        {
            get => _id ??= Guid.NewGuid();
            set => _id = value;
        }

        public Guid AccountId { get; set; }

        public string PortfolioName { get; set; }

        public string PortfolioType { get; set; }

        public string InvestmentObjective { get; set; }

        public string RiskTolerance { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime LastRebalanceDate { get; set; }

        public DateTime LastValuationDate { get; set; }

        public bool IsActive { get; set; }

        public PortfolioRiskMetric RiskMetrics { get; set; }

        public Restriction Restrictions { get; set; }

        public Performance Performance { get; set; }

        public Owner Owner { get; set; }

        public ICollection<Holding> Holdings { get; set; } = [];

        public AssetAllocation AssetAllocation { get; set; }

        public AccountDetail AccountDetails { get; set; }
    }
}