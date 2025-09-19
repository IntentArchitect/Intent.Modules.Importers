using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Finance.TradingPlatform.Portfolios;
using JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Portfolios;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Finance.TradingPlatform.Portfolios
{
    internal class StockDocument : IStockDocument
    {
        public decimal Percentage { get; set; }
        public decimal Value { get; set; }
        public List<SectorDocument> Sectors { get; set; } = default!;
        IReadOnlyList<ISectorDocument> IStockDocument.Sectors => Sectors;

        public Stock ToEntity(Stock? entity = default)
        {
            entity ??= new Stock();

            entity.Percentage = Percentage;
            entity.Value = Value;
            entity.Sectors = Sectors.Select(x => x.ToEntity()).ToList();

            return entity;
        }

        public StockDocument PopulateFromEntity(Stock entity)
        {
            Percentage = entity.Percentage;
            Value = entity.Value;
            Sectors = entity.Sectors.Select(x => SectorDocument.FromEntity(x)!).ToList();

            return this;
        }

        public static StockDocument? FromEntity(Stock? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new StockDocument().PopulateFromEntity(entity);
        }
    }
}