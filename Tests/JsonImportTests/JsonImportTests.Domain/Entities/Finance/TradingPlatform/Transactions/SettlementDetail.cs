using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Finance.TradingPlatform.Transactions
{
    public class SettlementDetail
    {
        public SettlementDetail()
        {
            SettlementCurrency = null!;
            SettlementMethod = null!;
            CustodianReference = null!;
            ClearingHouse = null!;
        }

        public string SettlementCurrency { get; set; }

        public decimal ExchangeRate { get; set; }

        public decimal SettlementAmount { get; set; }

        public string SettlementMethod { get; set; }

        public string CustodianReference { get; set; }

        public string ClearingHouse { get; set; }
    }
}