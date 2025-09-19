using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Healthcare.Staff
{
    public interface IPractitionerPersonalInfoDocument
    {
        string FirstName { get; }
        string LastName { get; }
        DateTime DateOfBirth { get; }
        string Gender { get; }
    }
}