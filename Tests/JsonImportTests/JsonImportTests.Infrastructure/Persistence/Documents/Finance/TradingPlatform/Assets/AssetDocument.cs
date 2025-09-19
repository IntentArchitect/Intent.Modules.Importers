using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Finance.TradingPlatform.Assets;
using JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Assets;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Finance.TradingPlatform.Assets
{
    internal class AssetDocument : IAssetDocument, ICosmosDBDocument<Asset, AssetDocument>
    {
        [JsonProperty("_etag")]
        protected string? _etag;
        private string? _type;
        [JsonProperty("type")]
        string IItem.Type
        {
            get => _type ??= GetType().GetNameForDocument();
            set => _type = value;
        }
        string? IItemWithEtag.Etag => _etag;
        public string Id { get; set; }
        public string Symbol { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string ISIN { get; set; } = default!;
        public string CUSIP { get; set; } = default!;
        public string AssetType { get; set; } = default!;
        public string AssetClass { get; set; } = default!;
        public string Sector { get; set; } = default!;
        public string Industry { get; set; } = default!;
        public string Exchange { get; set; } = default!;
        public string Currency { get; set; } = default!;
        public string Country { get; set; } = default!;
        public TradingInfoDocument TradingInfo { get; set; } = default!;
        ITradingInfoDocument IAssetDocument.TradingInfo => TradingInfo;
        public AssetRiskMetricDocument RiskMetrics { get; set; } = default!;
        IAssetRiskMetricDocument IAssetDocument.RiskMetrics => RiskMetrics;
        public PricingDatumDocument PricingData { get; set; } = default!;
        IPricingDatumDocument IAssetDocument.PricingData => PricingData;
        public AssetMetadataDocument Metadata { get; set; } = default!;
        IAssetMetadataDocument IAssetDocument.Metadata => Metadata;
        public FundamentalDocument Fundamentals { get; set; } = default!;
        IFundamentalDocument IAssetDocument.Fundamentals => Fundamentals;
        public FinancialStatementDocument FinancialStatements { get; set; } = default!;
        IFinancialStatementDocument IAssetDocument.FinancialStatements => FinancialStatements;
        public CompanyInfoDocument CompanyInfo { get; set; } = default!;
        ICompanyInfoDocument IAssetDocument.CompanyInfo => CompanyInfo;
        public AnalystDocument Analysts { get; set; } = default!;
        IAnalystDocument IAssetDocument.Analysts => Analysts;

        public Asset ToEntity(Asset? entity = default)
        {
            entity ??= new Asset();

            entity.Id = Guid.Parse(Id);
            entity.Symbol = Symbol ?? throw new Exception($"{nameof(entity.Symbol)} is null");
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");
            entity.ISIN = ISIN ?? throw new Exception($"{nameof(entity.ISIN)} is null");
            entity.CUSIP = CUSIP ?? throw new Exception($"{nameof(entity.CUSIP)} is null");
            entity.AssetType = AssetType ?? throw new Exception($"{nameof(entity.AssetType)} is null");
            entity.AssetClass = AssetClass ?? throw new Exception($"{nameof(entity.AssetClass)} is null");
            entity.Sector = Sector ?? throw new Exception($"{nameof(entity.Sector)} is null");
            entity.Industry = Industry ?? throw new Exception($"{nameof(entity.Industry)} is null");
            entity.Exchange = Exchange ?? throw new Exception($"{nameof(entity.Exchange)} is null");
            entity.Currency = Currency ?? throw new Exception($"{nameof(entity.Currency)} is null");
            entity.Country = Country ?? throw new Exception($"{nameof(entity.Country)} is null");
            entity.TradingInfo = TradingInfo.ToEntity() ?? throw new Exception($"{nameof(entity.TradingInfo)} is null");
            entity.RiskMetrics = RiskMetrics.ToEntity() ?? throw new Exception($"{nameof(entity.RiskMetrics)} is null");
            entity.PricingData = PricingData.ToEntity() ?? throw new Exception($"{nameof(entity.PricingData)} is null");
            entity.Metadata = Metadata.ToEntity() ?? throw new Exception($"{nameof(entity.Metadata)} is null");
            entity.Fundamentals = Fundamentals.ToEntity() ?? throw new Exception($"{nameof(entity.Fundamentals)} is null");
            entity.FinancialStatements = FinancialStatements.ToEntity() ?? throw new Exception($"{nameof(entity.FinancialStatements)} is null");
            entity.CompanyInfo = CompanyInfo.ToEntity() ?? throw new Exception($"{nameof(entity.CompanyInfo)} is null");
            entity.Analysts = Analysts.ToEntity() ?? throw new Exception($"{nameof(entity.Analysts)} is null");

            return entity;
        }

        public AssetDocument PopulateFromEntity(Asset entity, Func<string, string?> getEtag)
        {
            Id = entity.Id.ToString();
            Symbol = entity.Symbol;
            Name = entity.Name;
            ISIN = entity.ISIN;
            CUSIP = entity.CUSIP;
            AssetType = entity.AssetType;
            AssetClass = entity.AssetClass;
            Sector = entity.Sector;
            Industry = entity.Industry;
            Exchange = entity.Exchange;
            Currency = entity.Currency;
            Country = entity.Country;
            TradingInfo = TradingInfoDocument.FromEntity(entity.TradingInfo)!;
            RiskMetrics = AssetRiskMetricDocument.FromEntity(entity.RiskMetrics)!;
            PricingData = PricingDatumDocument.FromEntity(entity.PricingData)!;
            Metadata = AssetMetadataDocument.FromEntity(entity.Metadata)!;
            Fundamentals = FundamentalDocument.FromEntity(entity.Fundamentals)!;
            FinancialStatements = FinancialStatementDocument.FromEntity(entity.FinancialStatements)!;
            CompanyInfo = CompanyInfoDocument.FromEntity(entity.CompanyInfo)!;
            Analysts = AnalystDocument.FromEntity(entity.Analysts)!;

            _etag = _etag == null ? getEtag(((IItem)this).Id) : _etag;

            return this;
        }

        public static AssetDocument? FromEntity(Asset? entity, Func<string, string?> getEtag)
        {
            if (entity is null)
            {
                return null;
            }

            return new AssetDocument().PopulateFromEntity(entity, getEtag);
        }
    }
}