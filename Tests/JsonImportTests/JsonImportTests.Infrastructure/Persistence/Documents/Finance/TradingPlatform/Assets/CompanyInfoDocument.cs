using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Finance.TradingPlatform.Assets;
using JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Assets;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Finance.TradingPlatform.Assets
{
    internal class CompanyInfoDocument : ICompanyInfoDocument
    {
        public string CompanyName { get; set; } = default!;
        public decimal MarketCap { get; set; }
        public decimal EmployeeCount { get; set; }
        public string HeadquartersLocation { get; set; } = default!;
        public string Website { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string CEOName { get; set; } = default!;
        public decimal FoundedYear { get; set; }
        public string FiscalYearEnd { get; set; } = default!;

        public CompanyInfo ToEntity(CompanyInfo? entity = default)
        {
            entity ??= new CompanyInfo();

            entity.CompanyName = CompanyName ?? throw new Exception($"{nameof(entity.CompanyName)} is null");
            entity.MarketCap = MarketCap;
            entity.EmployeeCount = EmployeeCount;
            entity.HeadquartersLocation = HeadquartersLocation ?? throw new Exception($"{nameof(entity.HeadquartersLocation)} is null");
            entity.Website = Website ?? throw new Exception($"{nameof(entity.Website)} is null");
            entity.Description = Description ?? throw new Exception($"{nameof(entity.Description)} is null");
            entity.CEOName = CEOName ?? throw new Exception($"{nameof(entity.CEOName)} is null");
            entity.FoundedYear = FoundedYear;
            entity.FiscalYearEnd = FiscalYearEnd ?? throw new Exception($"{nameof(entity.FiscalYearEnd)} is null");

            return entity;
        }

        public CompanyInfoDocument PopulateFromEntity(CompanyInfo entity)
        {
            CompanyName = entity.CompanyName;
            MarketCap = entity.MarketCap;
            EmployeeCount = entity.EmployeeCount;
            HeadquartersLocation = entity.HeadquartersLocation;
            Website = entity.Website;
            Description = entity.Description;
            CEOName = entity.CEOName;
            FoundedYear = entity.FoundedYear;
            FiscalYearEnd = entity.FiscalYearEnd;

            return this;
        }

        public static CompanyInfoDocument? FromEntity(CompanyInfo? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new CompanyInfoDocument().PopulateFromEntity(entity);
        }
    }
}