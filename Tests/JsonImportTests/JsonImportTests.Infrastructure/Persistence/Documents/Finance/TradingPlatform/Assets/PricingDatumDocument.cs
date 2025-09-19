using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Finance.TradingPlatform.Assets;
using JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Assets;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Finance.TradingPlatform.Assets
{
    internal class PricingDatumDocument : IPricingDatumDocument
    {
        public decimal CurrentPrice { get; set; }
        public decimal PreviousClose { get; set; }
        public decimal DayHigh { get; set; }
        public decimal DayLow { get; set; }
        public decimal WeekHigh52 { get; set; }
        public decimal WeekLow52 { get; set; }
        public decimal Volume { get; set; }
        public decimal AverageVolume10Day { get; set; }
        public decimal AverageVolume3Month { get; set; }
        public decimal Beta { get; set; }
        public PricingDataVolatilityDocument Volatility { get; set; } = default!;
        IPricingDataVolatilityDocument IPricingDatumDocument.Volatility => Volatility;

        public PricingDatum ToEntity(PricingDatum? entity = default)
        {
            entity ??= new PricingDatum();

            entity.CurrentPrice = CurrentPrice;
            entity.PreviousClose = PreviousClose;
            entity.DayHigh = DayHigh;
            entity.DayLow = DayLow;
            entity.WeekHigh52 = WeekHigh52;
            entity.WeekLow52 = WeekLow52;
            entity.Volume = Volume;
            entity.AverageVolume10Day = AverageVolume10Day;
            entity.AverageVolume3Month = AverageVolume3Month;
            entity.Beta = Beta;
            entity.Volatility = Volatility.ToEntity() ?? throw new Exception($"{nameof(entity.Volatility)} is null");

            return entity;
        }

        public PricingDatumDocument PopulateFromEntity(PricingDatum entity)
        {
            CurrentPrice = entity.CurrentPrice;
            PreviousClose = entity.PreviousClose;
            DayHigh = entity.DayHigh;
            DayLow = entity.DayLow;
            WeekHigh52 = entity.WeekHigh52;
            WeekLow52 = entity.WeekLow52;
            Volume = entity.Volume;
            AverageVolume10Day = entity.AverageVolume10Day;
            AverageVolume3Month = entity.AverageVolume3Month;
            Beta = entity.Beta;
            Volatility = PricingDataVolatilityDocument.FromEntity(entity.Volatility)!;

            return this;
        }

        public static PricingDatumDocument? FromEntity(PricingDatum? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new PricingDatumDocument().PopulateFromEntity(entity);
        }
    }
}