using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Education.University.Students
{
    public interface IContactInfoEmergencyContactDocument
    {
        string Name { get; }
        string Relationship { get; }
        string Phone { get; }
        string Email { get; }
    }
}