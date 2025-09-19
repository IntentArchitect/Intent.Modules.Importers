using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Finance.TradingPlatform.Assets
{
    public class Asset
    {
        private Guid? _id;

        public Asset()
        {
            Symbol = null!;
            Name = null!;
            ISIN = null!;
            CUSIP = null!;
            AssetType = null!;
            AssetClass = null!;
            Sector = null!;
            Industry = null!;
            Exchange = null!;
            Currency = null!;
            Country = null!;
            TradingInfo = null!;
            RiskMetrics = null!;
            PricingData = null!;
            Metadata = null!;
            Fundamentals = null!;
            FinancialStatements = null!;
            CompanyInfo = null!;
            Analysts = null!;
        }

        public Guid Id
        {
            get => _id ??= Guid.NewGuid();
            set => _id = value;
        }

        public string Symbol { get; set; }

        public string Name { get; set; }

        public string ISIN { get; set; }

        public string CUSIP { get; set; }

        public string AssetType { get; set; }

        public string AssetClass { get; set; }

        public string Sector { get; set; }

        public string Industry { get; set; }

        public string Exchange { get; set; }

        public string Currency { get; set; }

        public string Country { get; set; }

        public TradingInfo TradingInfo { get; set; }

        public AssetRiskMetric RiskMetrics { get; set; }

        public PricingDatum PricingData { get; set; }

        public AssetMetadata Metadata { get; set; }

        public Fundamental Fundamentals { get; set; }

        public FinancialStatement FinancialStatements { get; set; }

        public CompanyInfo CompanyInfo { get; set; }

        public Analyst Analysts { get; set; }
    }
}