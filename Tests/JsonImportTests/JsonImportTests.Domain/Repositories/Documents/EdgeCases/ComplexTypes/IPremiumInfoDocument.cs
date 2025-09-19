using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.EdgeCases.ComplexTypes
{
    public interface IPremiumInfoDocument
    {
        string SubscriptionLevel { get; }
        IReadOnlyList<string> Features { get; }
        ILimitDocument Limits { get; }
        IBillingInfoDocument BillingInfo { get; }
    }
}