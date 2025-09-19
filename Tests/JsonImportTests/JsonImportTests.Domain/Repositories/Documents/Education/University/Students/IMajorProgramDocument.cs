using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Education.University.Students
{
    public interface IMajorProgramDocument
    {
        string Id { get; }
        Guid ProgramId { get; }
        string ProgramName { get; }
        string DegreeType { get; }
        string Major { get; }
        string Minor { get; }
        string Concentration { get; }
        DateTime DeclaredDate { get; }
    }
}