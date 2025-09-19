using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Portfolios
{
    public interface IOwnerDocument
    {
        Guid CustomerId { get; }
        string Name { get; }
        string Email { get; }
        string Phone { get; }
    }
}