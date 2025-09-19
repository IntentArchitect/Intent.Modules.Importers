using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Finance.TradingPlatform.Transactions;
using JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Transactions;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Finance.TradingPlatform.Transactions
{
    internal class InstrumentDocument : IInstrumentDocument
    {
        public string Symbol { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string AssetType { get; set; } = default!;
        public string ISIN { get; set; } = default!;
        public string Exchange { get; set; } = default!;
        public string Currency { get; set; } = default!;

        public Instrument ToEntity(Instrument? entity = default)
        {
            entity ??= new Instrument();

            entity.Symbol = Symbol ?? throw new Exception($"{nameof(entity.Symbol)} is null");
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");
            entity.AssetType = AssetType ?? throw new Exception($"{nameof(entity.AssetType)} is null");
            entity.ISIN = ISIN ?? throw new Exception($"{nameof(entity.ISIN)} is null");
            entity.Exchange = Exchange ?? throw new Exception($"{nameof(entity.Exchange)} is null");
            entity.Currency = Currency ?? throw new Exception($"{nameof(entity.Currency)} is null");

            return entity;
        }

        public InstrumentDocument PopulateFromEntity(Instrument entity)
        {
            Symbol = entity.Symbol;
            Name = entity.Name;
            AssetType = entity.AssetType;
            ISIN = entity.ISIN;
            Exchange = entity.Exchange;
            Currency = entity.Currency;

            return this;
        }

        public static InstrumentDocument? FromEntity(Instrument? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new InstrumentDocument().PopulateFromEntity(entity);
        }
    }
}