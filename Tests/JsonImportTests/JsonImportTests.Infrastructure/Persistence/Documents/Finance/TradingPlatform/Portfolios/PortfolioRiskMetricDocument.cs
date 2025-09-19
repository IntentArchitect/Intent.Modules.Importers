using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Finance.TradingPlatform.Portfolios;
using JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Portfolios;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Finance.TradingPlatform.Portfolios
{
    internal class PortfolioRiskMetricDocument : IPortfolioRiskMetricDocument
    {
        public decimal VaR95 { get; set; }
        public decimal ConditionalVaR { get; set; }
        public string RiskRating { get; set; } = default!;
        public string ConcentrationRisk { get; set; } = default!;
        public string LiquidityRisk { get; set; } = default!;

        public PortfolioRiskMetric ToEntity(PortfolioRiskMetric? entity = default)
        {
            entity ??= new PortfolioRiskMetric();

            entity.VaR95 = VaR95;
            entity.ConditionalVaR = ConditionalVaR;
            entity.RiskRating = RiskRating ?? throw new Exception($"{nameof(entity.RiskRating)} is null");
            entity.ConcentrationRisk = ConcentrationRisk ?? throw new Exception($"{nameof(entity.ConcentrationRisk)} is null");
            entity.LiquidityRisk = LiquidityRisk ?? throw new Exception($"{nameof(entity.LiquidityRisk)} is null");

            return entity;
        }

        public PortfolioRiskMetricDocument PopulateFromEntity(PortfolioRiskMetric entity)
        {
            VaR95 = entity.VaR95;
            ConditionalVaR = entity.ConditionalVaR;
            RiskRating = entity.RiskRating;
            ConcentrationRisk = entity.ConcentrationRisk;
            LiquidityRisk = entity.LiquidityRisk;

            return this;
        }

        public static PortfolioRiskMetricDocument? FromEntity(PortfolioRiskMetric? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new PortfolioRiskMetricDocument().PopulateFromEntity(entity);
        }
    }
}