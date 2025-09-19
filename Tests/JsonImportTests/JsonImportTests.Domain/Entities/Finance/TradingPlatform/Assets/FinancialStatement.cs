using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Finance.TradingPlatform.Assets
{
    public class FinancialStatement
    {
        public FinancialStatement()
        {
            ReportingFrequency = null!;
        }

        public DateTime LastReportingDate { get; set; }

        public string ReportingFrequency { get; set; }

        public decimal Revenue { get; set; }

        public decimal NetIncome { get; set; }

        public decimal TotalAssets { get; set; }

        public decimal TotalLiabilities { get; set; }

        public decimal ShareholdersEquity { get; set; }

        public decimal FreeCashFlow { get; set; }

        public decimal OperatingCashFlow { get; set; }
    }
}