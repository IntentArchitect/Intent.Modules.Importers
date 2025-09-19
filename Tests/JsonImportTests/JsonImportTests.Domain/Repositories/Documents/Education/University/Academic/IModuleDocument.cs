using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Education.University.Academic
{
    public interface IModuleDocument
    {
        Guid ModuleId { get; }
        string Title { get; }
        decimal Order { get; }
        IReadOnlyList<string> LearningObjectives { get; }
        IReadOnlyList<ITopicDocument> Topics { get; }
        IReadOnlyList<IAssignmentDocument> Assignments { get; }
    }
}