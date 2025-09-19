using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Healthcare.Staff
{
    public interface ICertificationDocument
    {
        string Id { get; }
        string Name { get; }
        string IssuingOrganization { get; }
        DateTime IssueDate { get; }
        DateTime ExpirationDate { get; }
        string CertificationNumber { get; }
    }
}