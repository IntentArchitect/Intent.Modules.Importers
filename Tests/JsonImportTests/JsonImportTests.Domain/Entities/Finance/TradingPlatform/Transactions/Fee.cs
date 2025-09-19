using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Finance.TradingPlatform.Transactions
{
    public class Fee
    {
        public Fee()
        {
            Currency = null!;
        }

        public decimal Commission { get; set; }

        public decimal RegulatoryFees { get; set; }

        public decimal ExchangeFees { get; set; }

        public decimal ClearingFees { get; set; }

        public decimal OtherFees { get; set; }

        public decimal TotalFees { get; set; }

        public string Currency { get; set; }
    }
}