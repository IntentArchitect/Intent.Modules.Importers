using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Finance.TradingPlatform.Assets;
using JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Assets;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Finance.TradingPlatform.Assets
{
    internal class FinancialStatementDocument : IFinancialStatementDocument
    {
        public DateTime LastReportingDate { get; set; }
        public string ReportingFrequency { get; set; } = default!;
        public decimal Revenue { get; set; }
        public decimal NetIncome { get; set; }
        public decimal TotalAssets { get; set; }
        public decimal TotalLiabilities { get; set; }
        public decimal ShareholdersEquity { get; set; }
        public decimal FreeCashFlow { get; set; }
        public decimal OperatingCashFlow { get; set; }

        public FinancialStatement ToEntity(FinancialStatement? entity = default)
        {
            entity ??= new FinancialStatement();

            entity.LastReportingDate = LastReportingDate;
            entity.ReportingFrequency = ReportingFrequency ?? throw new Exception($"{nameof(entity.ReportingFrequency)} is null");
            entity.Revenue = Revenue;
            entity.NetIncome = NetIncome;
            entity.TotalAssets = TotalAssets;
            entity.TotalLiabilities = TotalLiabilities;
            entity.ShareholdersEquity = ShareholdersEquity;
            entity.FreeCashFlow = FreeCashFlow;
            entity.OperatingCashFlow = OperatingCashFlow;

            return entity;
        }

        public FinancialStatementDocument PopulateFromEntity(FinancialStatement entity)
        {
            LastReportingDate = entity.LastReportingDate;
            ReportingFrequency = entity.ReportingFrequency;
            Revenue = entity.Revenue;
            NetIncome = entity.NetIncome;
            TotalAssets = entity.TotalAssets;
            TotalLiabilities = entity.TotalLiabilities;
            ShareholdersEquity = entity.ShareholdersEquity;
            FreeCashFlow = entity.FreeCashFlow;
            OperatingCashFlow = entity.OperatingCashFlow;

            return this;
        }

        public static FinancialStatementDocument? FromEntity(FinancialStatement? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new FinancialStatementDocument().PopulateFromEntity(entity);
        }
    }
}