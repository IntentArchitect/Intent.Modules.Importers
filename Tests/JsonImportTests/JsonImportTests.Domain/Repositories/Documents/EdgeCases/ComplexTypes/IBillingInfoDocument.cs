using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.EdgeCases.ComplexTypes
{
    public interface IBillingInfoDocument
    {
        string Plan { get; }
        decimal Amount { get; }
        string Currency { get; }
        DateTime NextBillingDate { get; }
        IPaymentMethodDocument PaymentMethod { get; }
    }
}