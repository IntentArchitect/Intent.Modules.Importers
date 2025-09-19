using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.EdgeCases.ComplexTypes
{
    public interface IPaymentMethodDocument
    {
        string Type { get; }
        string Last4 { get; }
        decimal ExpiryMonth { get; }
        decimal ExpiryYear { get; }
    }
}