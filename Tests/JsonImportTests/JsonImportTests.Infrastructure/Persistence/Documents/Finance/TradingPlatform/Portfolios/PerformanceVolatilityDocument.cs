using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Finance.TradingPlatform.Portfolios;
using JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Portfolios;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Finance.TradingPlatform.Portfolios
{
    internal class PerformanceVolatilityDocument : IPerformanceVolatilityDocument
    {
        public decimal Beta { get; set; }
        public decimal StandardDeviation { get; set; }
        public decimal SharpeRatio { get; set; }
        public decimal MaxDrawdown { get; set; }

        public PerformanceVolatility ToEntity(PerformanceVolatility? entity = default)
        {
            entity ??= new PerformanceVolatility();

            entity.Beta = Beta;
            entity.StandardDeviation = StandardDeviation;
            entity.SharpeRatio = SharpeRatio;
            entity.MaxDrawdown = MaxDrawdown;

            return entity;
        }

        public PerformanceVolatilityDocument PopulateFromEntity(PerformanceVolatility entity)
        {
            Beta = entity.Beta;
            StandardDeviation = entity.StandardDeviation;
            SharpeRatio = entity.SharpeRatio;
            MaxDrawdown = entity.MaxDrawdown;

            return this;
        }

        public static PerformanceVolatilityDocument? FromEntity(PerformanceVolatility? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new PerformanceVolatilityDocument().PopulateFromEntity(entity);
        }
    }
}