using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Education.University.Academic
{
    public interface ICorequisiteDocument
    {
        string Id { get; }
        Guid CourseId { get; }
        string CourseCode { get; }
        string CourseName { get; }
        bool CanBeTakenConcurrently { get; }
    }
}