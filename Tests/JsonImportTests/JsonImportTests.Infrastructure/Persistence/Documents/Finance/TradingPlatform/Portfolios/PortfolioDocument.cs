using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Finance.TradingPlatform.Portfolios;
using JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Portfolios;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Finance.TradingPlatform.Portfolios
{
    internal class PortfolioDocument : IPortfolioDocument, ICosmosDBDocument<Portfolio, PortfolioDocument>
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
        public Guid AccountId { get; set; }
        public string PortfolioName { get; set; } = default!;
        public string PortfolioType { get; set; } = default!;
        public string InvestmentObjective { get; set; } = default!;
        public string RiskTolerance { get; set; } = default!;
        public DateTime CreatedDate { get; set; }
        public DateTime LastRebalanceDate { get; set; }
        public DateTime LastValuationDate { get; set; }
        public bool IsActive { get; set; }
        public PortfolioRiskMetricDocument RiskMetrics { get; set; } = default!;
        IPortfolioRiskMetricDocument IPortfolioDocument.RiskMetrics => RiskMetrics;
        public RestrictionDocument Restrictions { get; set; } = default!;
        IRestrictionDocument IPortfolioDocument.Restrictions => Restrictions;
        public PerformanceDocument Performance { get; set; } = default!;
        IPerformanceDocument IPortfolioDocument.Performance => Performance;
        public OwnerDocument Owner { get; set; } = default!;
        IOwnerDocument IPortfolioDocument.Owner => Owner;
        public List<HoldingDocument> Holdings { get; set; } = default!;
        IReadOnlyList<IHoldingDocument> IPortfolioDocument.Holdings => Holdings;
        public AssetAllocationDocument AssetAllocation { get; set; } = default!;
        IAssetAllocationDocument IPortfolioDocument.AssetAllocation => AssetAllocation;
        public AccountDetailDocument AccountDetails { get; set; } = default!;
        IAccountDetailDocument IPortfolioDocument.AccountDetails => AccountDetails;

        public Portfolio ToEntity(Portfolio? entity = default)
        {
            entity ??= new Portfolio();

            entity.Id = Guid.Parse(Id);
            entity.AccountId = AccountId;
            entity.PortfolioName = PortfolioName ?? throw new Exception($"{nameof(entity.PortfolioName)} is null");
            entity.PortfolioType = PortfolioType ?? throw new Exception($"{nameof(entity.PortfolioType)} is null");
            entity.InvestmentObjective = InvestmentObjective ?? throw new Exception($"{nameof(entity.InvestmentObjective)} is null");
            entity.RiskTolerance = RiskTolerance ?? throw new Exception($"{nameof(entity.RiskTolerance)} is null");
            entity.CreatedDate = CreatedDate;
            entity.LastRebalanceDate = LastRebalanceDate;
            entity.LastValuationDate = LastValuationDate;
            entity.IsActive = IsActive;
            entity.RiskMetrics = RiskMetrics.ToEntity() ?? throw new Exception($"{nameof(entity.RiskMetrics)} is null");
            entity.Restrictions = Restrictions.ToEntity() ?? throw new Exception($"{nameof(entity.Restrictions)} is null");
            entity.Performance = Performance.ToEntity() ?? throw new Exception($"{nameof(entity.Performance)} is null");
            entity.Owner = Owner.ToEntity() ?? throw new Exception($"{nameof(entity.Owner)} is null");
            entity.Holdings = Holdings.Select(x => x.ToEntity()).ToList();
            entity.AssetAllocation = AssetAllocation.ToEntity() ?? throw new Exception($"{nameof(entity.AssetAllocation)} is null");
            entity.AccountDetails = AccountDetails.ToEntity() ?? throw new Exception($"{nameof(entity.AccountDetails)} is null");

            return entity;
        }

        public PortfolioDocument PopulateFromEntity(Portfolio entity, Func<string, string?> getEtag)
        {
            Id = entity.Id.ToString();
            AccountId = entity.AccountId;
            PortfolioName = entity.PortfolioName;
            PortfolioType = entity.PortfolioType;
            InvestmentObjective = entity.InvestmentObjective;
            RiskTolerance = entity.RiskTolerance;
            CreatedDate = entity.CreatedDate;
            LastRebalanceDate = entity.LastRebalanceDate;
            LastValuationDate = entity.LastValuationDate;
            IsActive = entity.IsActive;
            RiskMetrics = PortfolioRiskMetricDocument.FromEntity(entity.RiskMetrics)!;
            Restrictions = RestrictionDocument.FromEntity(entity.Restrictions)!;
            Performance = PerformanceDocument.FromEntity(entity.Performance)!;
            Owner = OwnerDocument.FromEntity(entity.Owner)!;
            Holdings = entity.Holdings.Select(x => HoldingDocument.FromEntity(x)!).ToList();
            AssetAllocation = AssetAllocationDocument.FromEntity(entity.AssetAllocation)!;
            AccountDetails = AccountDetailDocument.FromEntity(entity.AccountDetails)!;

            _etag = _etag == null ? getEtag(((IItem)this).Id) : _etag;

            return this;
        }

        public static PortfolioDocument? FromEntity(Portfolio? entity, Func<string, string?> getEtag)
        {
            if (entity is null)
            {
                return null;
            }

            return new PortfolioDocument().PopulateFromEntity(entity, getEtag);
        }
    }
}