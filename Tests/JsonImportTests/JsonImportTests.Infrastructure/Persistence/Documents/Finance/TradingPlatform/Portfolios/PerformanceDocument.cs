using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Finance.TradingPlatform.Portfolios;
using JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Portfolios;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Finance.TradingPlatform.Portfolios
{
    internal class PerformanceDocument : IPerformanceDocument
    {
        public decimal TotalValue { get; set; }
        public decimal CashValue { get; set; }
        public decimal InvestedValue { get; set; }
        public decimal TotalGainLoss { get; set; }
        public decimal TotalGainLossPercentage { get; set; }
        public decimal DayChange { get; set; }
        public decimal DayChangePercentage { get; set; }
        public decimal YearToDateReturn { get; set; }
        public decimal OneYearReturn { get; set; }
        public decimal ThreeYearReturn { get; set; }
        public decimal FiveYearReturn { get; set; }
        public decimal InceptionReturn { get; set; }
        public PerformanceVolatilityDocument Volatility { get; set; } = default!;
        IPerformanceVolatilityDocument IPerformanceDocument.Volatility => Volatility;

        public Performance ToEntity(Performance? entity = default)
        {
            entity ??= new Performance();

            entity.TotalValue = TotalValue;
            entity.CashValue = CashValue;
            entity.InvestedValue = InvestedValue;
            entity.TotalGainLoss = TotalGainLoss;
            entity.TotalGainLossPercentage = TotalGainLossPercentage;
            entity.DayChange = DayChange;
            entity.DayChangePercentage = DayChangePercentage;
            entity.YearToDateReturn = YearToDateReturn;
            entity.OneYearReturn = OneYearReturn;
            entity.ThreeYearReturn = ThreeYearReturn;
            entity.FiveYearReturn = FiveYearReturn;
            entity.InceptionReturn = InceptionReturn;
            entity.Volatility = Volatility.ToEntity() ?? throw new Exception($"{nameof(entity.Volatility)} is null");

            return entity;
        }

        public PerformanceDocument PopulateFromEntity(Performance entity)
        {
            TotalValue = entity.TotalValue;
            CashValue = entity.CashValue;
            InvestedValue = entity.InvestedValue;
            TotalGainLoss = entity.TotalGainLoss;
            TotalGainLossPercentage = entity.TotalGainLossPercentage;
            DayChange = entity.DayChange;
            DayChangePercentage = entity.DayChangePercentage;
            YearToDateReturn = entity.YearToDateReturn;
            OneYearReturn = entity.OneYearReturn;
            ThreeYearReturn = entity.ThreeYearReturn;
            FiveYearReturn = entity.FiveYearReturn;
            InceptionReturn = entity.InceptionReturn;
            Volatility = PerformanceVolatilityDocument.FromEntity(entity.Volatility)!;

            return this;
        }

        public static PerformanceDocument? FromEntity(Performance? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new PerformanceDocument().PopulateFromEntity(entity);
        }
    }
}