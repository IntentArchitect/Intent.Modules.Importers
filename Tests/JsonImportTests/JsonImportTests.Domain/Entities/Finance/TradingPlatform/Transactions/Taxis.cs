using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Finance.TradingPlatform.Transactions
{
    public class Taxis
    {
        public Taxis()
        {
            TaxCurrency = null!;
        }

        public decimal TransactionTax { get; set; }

        public decimal StampDuty { get; set; }

        public decimal WithholdingTax { get; set; }

        public decimal TotalTaxes { get; set; }

        public string TaxCurrency { get; set; }
    }
}