using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Education.University.Students
{
    public interface IOrganizationDocument
    {
        Guid OrganizationId { get; }
        string Name { get; }
        string Role { get; }
        DateTime JoinDate { get; }
        bool IsActive { get; }
    }
}