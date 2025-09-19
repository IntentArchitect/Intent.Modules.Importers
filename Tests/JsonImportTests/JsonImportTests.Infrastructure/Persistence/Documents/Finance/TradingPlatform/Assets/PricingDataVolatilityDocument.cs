using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Finance.TradingPlatform.Assets;
using JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Assets;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Finance.TradingPlatform.Assets
{
    internal class PricingDataVolatilityDocument : IPricingDataVolatilityDocument
    {
        public decimal ImpliedVolatility { get; set; }
        public decimal HistoricalVolatility30Day { get; set; }
        public decimal HistoricalVolatility90Day { get; set; }

        public PricingDataVolatility ToEntity(PricingDataVolatility? entity = default)
        {
            entity ??= new PricingDataVolatility();

            entity.ImpliedVolatility = ImpliedVolatility;
            entity.HistoricalVolatility30Day = HistoricalVolatility30Day;
            entity.HistoricalVolatility90Day = HistoricalVolatility90Day;

            return entity;
        }

        public PricingDataVolatilityDocument PopulateFromEntity(PricingDataVolatility entity)
        {
            ImpliedVolatility = entity.ImpliedVolatility;
            HistoricalVolatility30Day = entity.HistoricalVolatility30Day;
            HistoricalVolatility90Day = entity.HistoricalVolatility90Day;

            return this;
        }

        public static PricingDataVolatilityDocument? FromEntity(PricingDataVolatility? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new PricingDataVolatilityDocument().PopulateFromEntity(entity);
        }
    }
}