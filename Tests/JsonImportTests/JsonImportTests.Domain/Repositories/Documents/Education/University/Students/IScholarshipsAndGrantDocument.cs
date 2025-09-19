using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Education.University.Students
{
    public interface IScholarshipsAndGrantDocument
    {
        string Id { get; }
        Guid AwardId { get; }
        string Name { get; }
        string Type { get; }
        decimal Amount { get; }
        string Semester { get; }
        decimal Year { get; }
        bool Renewable { get; }
        string Requirements { get; }
    }
}