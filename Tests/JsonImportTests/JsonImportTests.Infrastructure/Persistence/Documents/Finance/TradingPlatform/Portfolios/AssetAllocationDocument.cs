using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Finance.TradingPlatform.Portfolios;
using JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Portfolios;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Finance.TradingPlatform.Portfolios
{
    internal class AssetAllocationDocument : IAssetAllocationDocument
    {
        public StockDocument Stocks { get; set; } = default!;
        IStockDocument IAssetAllocationDocument.Stocks => Stocks;
        public OtherDocument Other { get; set; } = default!;
        IOtherDocument IAssetAllocationDocument.Other => Other;
        public CashDocument Cash { get; set; } = default!;
        ICashDocument IAssetAllocationDocument.Cash => Cash;
        public BondDocument Bonds { get; set; } = default!;
        IBondDocument IAssetAllocationDocument.Bonds => Bonds;

        public AssetAllocation ToEntity(AssetAllocation? entity = default)
        {
            entity ??= new AssetAllocation();
            entity.Stocks = Stocks.ToEntity() ?? throw new Exception($"{nameof(entity.Stocks)} is null");
            entity.Other = Other.ToEntity() ?? throw new Exception($"{nameof(entity.Other)} is null");
            entity.Cash = Cash.ToEntity() ?? throw new Exception($"{nameof(entity.Cash)} is null");
            entity.Bonds = Bonds.ToEntity() ?? throw new Exception($"{nameof(entity.Bonds)} is null");

            return entity;
        }

        public AssetAllocationDocument PopulateFromEntity(AssetAllocation entity)
        {
            Stocks = StockDocument.FromEntity(entity.Stocks)!;
            Other = OtherDocument.FromEntity(entity.Other)!;
            Cash = CashDocument.FromEntity(entity.Cash)!;
            Bonds = BondDocument.FromEntity(entity.Bonds)!;

            return this;
        }

        public static AssetAllocationDocument? FromEntity(AssetAllocation? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new AssetAllocationDocument().PopulateFromEntity(entity);
        }
    }
}