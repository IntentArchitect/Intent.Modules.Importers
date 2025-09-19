using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Finance.TradingPlatform.Assets;
using JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Assets;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Finance.TradingPlatform.Assets
{
    internal class FundamentalDocument : IFundamentalDocument
    {
        public decimal MarketCapitalization { get; set; }
        public decimal EnterpriseValue { get; set; }
        public decimal PERatio { get; set; }
        public decimal EarningsPerShare { get; set; }
        public decimal BookValuePerShare { get; set; }
        public decimal PriceToBook { get; set; }
        public decimal PriceToSales { get; set; }
        public decimal DividendPerShare { get; set; }
        public decimal DividendYield { get; set; }
        public decimal PayoutRatio { get; set; }
        public decimal ReturnOnEquity { get; set; }
        public decimal ReturnOnAssets { get; set; }
        public decimal DebtToEquity { get; set; }
        public decimal CurrentRatio { get; set; }
        public decimal QuickRatio { get; set; }

        public Fundamental ToEntity(Fundamental? entity = default)
        {
            entity ??= new Fundamental();

            entity.MarketCapitalization = MarketCapitalization;
            entity.EnterpriseValue = EnterpriseValue;
            entity.PERatio = PERatio;
            entity.EarningsPerShare = EarningsPerShare;
            entity.BookValuePerShare = BookValuePerShare;
            entity.PriceToBook = PriceToBook;
            entity.PriceToSales = PriceToSales;
            entity.DividendPerShare = DividendPerShare;
            entity.DividendYield = DividendYield;
            entity.PayoutRatio = PayoutRatio;
            entity.ReturnOnEquity = ReturnOnEquity;
            entity.ReturnOnAssets = ReturnOnAssets;
            entity.DebtToEquity = DebtToEquity;
            entity.CurrentRatio = CurrentRatio;
            entity.QuickRatio = QuickRatio;

            return entity;
        }

        public FundamentalDocument PopulateFromEntity(Fundamental entity)
        {
            MarketCapitalization = entity.MarketCapitalization;
            EnterpriseValue = entity.EnterpriseValue;
            PERatio = entity.PERatio;
            EarningsPerShare = entity.EarningsPerShare;
            BookValuePerShare = entity.BookValuePerShare;
            PriceToBook = entity.PriceToBook;
            PriceToSales = entity.PriceToSales;
            DividendPerShare = entity.DividendPerShare;
            DividendYield = entity.DividendYield;
            PayoutRatio = entity.PayoutRatio;
            ReturnOnEquity = entity.ReturnOnEquity;
            ReturnOnAssets = entity.ReturnOnAssets;
            DebtToEquity = entity.DebtToEquity;
            CurrentRatio = entity.CurrentRatio;
            QuickRatio = entity.QuickRatio;

            return this;
        }

        public static FundamentalDocument? FromEntity(Fundamental? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new FundamentalDocument().PopulateFromEntity(entity);
        }
    }
}