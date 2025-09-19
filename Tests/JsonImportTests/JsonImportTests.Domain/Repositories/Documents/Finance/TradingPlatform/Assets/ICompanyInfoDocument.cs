using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Assets
{
    public interface ICompanyInfoDocument
    {
        string CompanyName { get; }
        decimal MarketCap { get; }
        decimal EmployeeCount { get; }
        string HeadquartersLocation { get; }
        string Website { get; }
        string Description { get; }
        string CEOName { get; }
        decimal FoundedYear { get; }
        string FiscalYearEnd { get; }
    }
}