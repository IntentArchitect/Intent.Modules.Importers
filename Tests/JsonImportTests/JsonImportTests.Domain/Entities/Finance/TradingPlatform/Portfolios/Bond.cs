using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Finance.TradingPlatform.Portfolios
{
    public class Bond
    {
        public decimal Percentage { get; set; }

        public decimal Value { get; set; }

        public ICollection<Type> Types { get; set; } = [];
    }
}