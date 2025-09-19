using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Finance.TradingPlatform.Transactions
{
    public class RiskDatum
    {
        public RiskDatum()
        {
            RiskClass = null!;
        }

        public decimal VaRImpact { get; set; }

        public decimal DeltaEquivalent { get; set; }

        public decimal NotionalAmount { get; set; }

        public string RiskClass { get; set; }

        public decimal ConcentrationLimit { get; set; }

        public decimal PositionLimit { get; set; }
    }
}