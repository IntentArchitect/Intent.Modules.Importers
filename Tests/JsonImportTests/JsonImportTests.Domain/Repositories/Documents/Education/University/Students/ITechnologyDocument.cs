using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Education.University.Students
{
    public interface ITechnologyDocument
    {
        string UniversityEmail { get; }
        bool StudentPortalAccess { get; }
        bool LibraryAccess { get; }
        IReadOnlyList<IITServiceDocument> ITServices { get; }
    }
}