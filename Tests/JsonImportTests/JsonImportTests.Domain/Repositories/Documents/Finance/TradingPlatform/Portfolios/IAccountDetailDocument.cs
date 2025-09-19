using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Portfolios
{
    public interface IAccountDetailDocument
    {
        string AccountNumber { get; }
        string AccountType { get; }
        string TaxStatus { get; }
        string BaseCurrency { get; }
        DateTime OpenDate { get; }
        string Status { get; }
    }
}