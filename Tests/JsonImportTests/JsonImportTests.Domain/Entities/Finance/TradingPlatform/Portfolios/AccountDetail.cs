using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Finance.TradingPlatform.Portfolios
{
    public class AccountDetail
    {
        public AccountDetail()
        {
            AccountNumber = null!;
            AccountType = null!;
            TaxStatus = null!;
            BaseCurrency = null!;
            Status = null!;
        }

        public string AccountNumber { get; set; }

        public string AccountType { get; set; }

        public string TaxStatus { get; set; }

        public string BaseCurrency { get; set; }

        public DateTime OpenDate { get; set; }

        public string Status { get; set; }
    }
}