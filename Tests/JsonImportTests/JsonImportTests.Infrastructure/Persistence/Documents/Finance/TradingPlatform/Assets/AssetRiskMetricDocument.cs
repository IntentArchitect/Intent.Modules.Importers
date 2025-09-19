using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Finance.TradingPlatform.Assets;
using JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Assets;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Finance.TradingPlatform.Assets
{
    internal class AssetRiskMetricDocument : IAssetRiskMetricDocument
    {
        public decimal ESGScore { get; set; }
        public string LiquidityRisk { get; set; } = default!;
        public string ConcentrationRisk { get; set; } = default!;
        public string RegulatoryRisk { get; set; } = default!;
        public CreditRatingDocument CreditRating { get; set; } = default!;
        ICreditRatingDocument IAssetRiskMetricDocument.CreditRating => CreditRating;

        public AssetRiskMetric ToEntity(AssetRiskMetric? entity = default)
        {
            entity ??= new AssetRiskMetric();

            entity.ESGScore = ESGScore;
            entity.LiquidityRisk = LiquidityRisk ?? throw new Exception($"{nameof(entity.LiquidityRisk)} is null");
            entity.ConcentrationRisk = ConcentrationRisk ?? throw new Exception($"{nameof(entity.ConcentrationRisk)} is null");
            entity.RegulatoryRisk = RegulatoryRisk ?? throw new Exception($"{nameof(entity.RegulatoryRisk)} is null");
            entity.CreditRating = CreditRating.ToEntity() ?? throw new Exception($"{nameof(entity.CreditRating)} is null");

            return entity;
        }

        public AssetRiskMetricDocument PopulateFromEntity(AssetRiskMetric entity)
        {
            ESGScore = entity.ESGScore;
            LiquidityRisk = entity.LiquidityRisk;
            ConcentrationRisk = entity.ConcentrationRisk;
            RegulatoryRisk = entity.RegulatoryRisk;
            CreditRating = CreditRatingDocument.FromEntity(entity.CreditRating)!;

            return this;
        }

        public static AssetRiskMetricDocument? FromEntity(AssetRiskMetric? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new AssetRiskMetricDocument().PopulateFromEntity(entity);
        }
    }
}