using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Education.University.Academic
{
    public interface IOfferingsInstructorDocument
    {
        Guid InstructorId { get; }
        string Name { get; }
        string Email { get; }
        string Title { get; }
        string Department { get; }
    }
}