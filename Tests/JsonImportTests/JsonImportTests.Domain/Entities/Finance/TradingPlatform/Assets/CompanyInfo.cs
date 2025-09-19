using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Finance.TradingPlatform.Assets
{
    public class CompanyInfo
    {
        public CompanyInfo()
        {
            CompanyName = null!;
            HeadquartersLocation = null!;
            Website = null!;
            Description = null!;
            CEOName = null!;
            FiscalYearEnd = null!;
        }

        public string CompanyName { get; set; }

        public decimal MarketCap { get; set; }

        public decimal EmployeeCount { get; set; }

        public string HeadquartersLocation { get; set; }

        public string Website { get; set; }

        public string Description { get; set; }

        public string CEOName { get; set; }

        public decimal FoundedYear { get; set; }

        public string FiscalYearEnd { get; set; }
    }
}