using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Finance.TradingPlatform.Assets;
using JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Assets;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Finance.TradingPlatform.Assets
{
    internal class TradingHourDocument : ITradingHourDocument
    {
        public string MarketOpen { get; set; } = default!;
        public string MarketClose { get; set; } = default!;
        public string TimeZone { get; set; } = default!;

        public TradingHour ToEntity(TradingHour? entity = default)
        {
            entity ??= new TradingHour();

            entity.MarketOpen = MarketOpen ?? throw new Exception($"{nameof(entity.MarketOpen)} is null");
            entity.MarketClose = MarketClose ?? throw new Exception($"{nameof(entity.MarketClose)} is null");
            entity.TimeZone = TimeZone ?? throw new Exception($"{nameof(entity.TimeZone)} is null");

            return entity;
        }

        public TradingHourDocument PopulateFromEntity(TradingHour entity)
        {
            MarketOpen = entity.MarketOpen;
            MarketClose = entity.MarketClose;
            TimeZone = entity.TimeZone;

            return this;
        }

        public static TradingHourDocument? FromEntity(TradingHour? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new TradingHourDocument().PopulateFromEntity(entity);
        }
    }
}