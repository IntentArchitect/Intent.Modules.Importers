using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Education.University.EducationEnrollment
{
    public interface IIncompleteStatusDocument
    {
        bool IsIncomplete { get; }
        object IncompleteReason { get; }
        object CompletionDeadline { get; }
        IReadOnlyList<object> ExtensionRequests { get; }
    }
}