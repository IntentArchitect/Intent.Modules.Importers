using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Education.University.Students
{
    public interface IAcademicAdvisorDocument
    {
        Guid AdvisorId { get; }
        string Name { get; }
        string Email { get; }
        string Department { get; }
        DateTime AssignedDate { get; }
    }
}