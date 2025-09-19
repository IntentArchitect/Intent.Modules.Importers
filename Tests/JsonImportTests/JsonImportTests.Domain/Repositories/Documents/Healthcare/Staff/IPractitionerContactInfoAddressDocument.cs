using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Healthcare.Staff
{
    public interface IPractitionerContactInfoAddressDocument
    {
        string Street { get; }
        string City { get; }
        string State { get; }
        string ZipCode { get; }
    }
}