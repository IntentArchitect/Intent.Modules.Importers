using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Finance.TradingPlatform.Assets;
using JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Assets;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Finance.TradingPlatform.Assets
{
    internal class TradingInfoDocument : ITradingInfoDocument
    {
        public string PrimaryExchange { get; set; } = default!;
        public List<string> SecondaryExchanges { get; set; } = default!;
        IReadOnlyList<string> ITradingInfoDocument.SecondaryExchanges => SecondaryExchanges;
        public decimal TickSize { get; set; }
        public decimal LotSize { get; set; }
        public bool IsShortable { get; set; }
        public bool IsMarginable { get; set; }
        public bool DividendEligible { get; set; }
        public TradingHourDocument TradingHours { get; set; } = default!;
        ITradingHourDocument ITradingInfoDocument.TradingHours => TradingHours;

        public TradingInfo ToEntity(TradingInfo? entity = default)
        {
            entity ??= new TradingInfo();

            entity.PrimaryExchange = PrimaryExchange ?? throw new Exception($"{nameof(entity.PrimaryExchange)} is null");
            entity.SecondaryExchanges = SecondaryExchanges ?? throw new Exception($"{nameof(entity.SecondaryExchanges)} is null");
            entity.TickSize = TickSize;
            entity.LotSize = LotSize;
            entity.IsShortable = IsShortable;
            entity.IsMarginable = IsMarginable;
            entity.DividendEligible = DividendEligible;
            entity.TradingHours = TradingHours.ToEntity() ?? throw new Exception($"{nameof(entity.TradingHours)} is null");

            return entity;
        }

        public TradingInfoDocument PopulateFromEntity(TradingInfo entity)
        {
            PrimaryExchange = entity.PrimaryExchange;
            SecondaryExchanges = entity.SecondaryExchanges.ToList();
            TickSize = entity.TickSize;
            LotSize = entity.LotSize;
            IsShortable = entity.IsShortable;
            IsMarginable = entity.IsMarginable;
            DividendEligible = entity.DividendEligible;
            TradingHours = TradingHourDocument.FromEntity(entity.TradingHours)!;

            return this;
        }

        public static TradingInfoDocument? FromEntity(TradingInfo? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new TradingInfoDocument().PopulateFromEntity(entity);
        }
    }
}