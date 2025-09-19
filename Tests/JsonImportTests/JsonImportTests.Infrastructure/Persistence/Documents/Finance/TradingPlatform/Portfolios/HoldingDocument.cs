using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Finance.TradingPlatform.Portfolios;
using JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Portfolios;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Finance.TradingPlatform.Portfolios
{
    internal class HoldingDocument : IHoldingDocument
    {
        public Guid Id { get; set; }
        public Guid AssetId { get; set; }
        public string Symbol { get; set; } = default!;
        public string AssetType { get; set; } = default!;
        public decimal Quantity { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal CurrentPrice { get; set; }
        public decimal MarketValue { get; set; }
        public decimal UnrealizedGainLoss { get; set; }
        public decimal PercentageGainLoss { get; set; }
        public decimal PercentOfPortfolio { get; set; }
        public DateTime FirstPurchaseDate { get; set; }
        public DateTime LastTransactionDate { get; set; }
        public decimal DividendYield { get; set; }
        public decimal AccruedDividends { get; set; }

        public Holding ToEntity(Holding? entity = default)
        {
            entity ??= new Holding();

            entity.Id = Id;
            entity.AssetId = AssetId;
            entity.Symbol = Symbol ?? throw new Exception($"{nameof(entity.Symbol)} is null");
            entity.AssetType = AssetType ?? throw new Exception($"{nameof(entity.AssetType)} is null");
            entity.Quantity = Quantity;
            entity.AveragePrice = AveragePrice;
            entity.CurrentPrice = CurrentPrice;
            entity.MarketValue = MarketValue;
            entity.UnrealizedGainLoss = UnrealizedGainLoss;
            entity.PercentageGainLoss = PercentageGainLoss;
            entity.PercentOfPortfolio = PercentOfPortfolio;
            entity.FirstPurchaseDate = FirstPurchaseDate;
            entity.LastTransactionDate = LastTransactionDate;
            entity.DividendYield = DividendYield;
            entity.AccruedDividends = AccruedDividends;

            return entity;
        }

        public HoldingDocument PopulateFromEntity(Holding entity)
        {
            Id = entity.Id;
            AssetId = entity.AssetId;
            Symbol = entity.Symbol;
            AssetType = entity.AssetType;
            Quantity = entity.Quantity;
            AveragePrice = entity.AveragePrice;
            CurrentPrice = entity.CurrentPrice;
            MarketValue = entity.MarketValue;
            UnrealizedGainLoss = entity.UnrealizedGainLoss;
            PercentageGainLoss = entity.PercentageGainLoss;
            PercentOfPortfolio = entity.PercentOfPortfolio;
            FirstPurchaseDate = entity.FirstPurchaseDate;
            LastTransactionDate = entity.LastTransactionDate;
            DividendYield = entity.DividendYield;
            AccruedDividends = entity.AccruedDividends;

            return this;
        }

        public static HoldingDocument? FromEntity(Holding? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new HoldingDocument().PopulateFromEntity(entity);
        }
    }
}