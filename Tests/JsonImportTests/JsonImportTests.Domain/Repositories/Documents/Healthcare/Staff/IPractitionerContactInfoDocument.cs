using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Healthcare.Staff
{
    public interface IPractitionerContactInfoDocument
    {
        string Phone { get; }
        string Email { get; }
        IPractitionerContactInfoAddressDocument Address { get; }
    }
}