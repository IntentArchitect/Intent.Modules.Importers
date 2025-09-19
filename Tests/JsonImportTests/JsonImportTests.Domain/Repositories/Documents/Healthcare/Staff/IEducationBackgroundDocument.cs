using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Healthcare.Staff
{
    public interface IEducationBackgroundDocument
    {
        string Id { get; }
        string Institution { get; }
        string Degree { get; }
        string FieldOfStudy { get; }
        decimal GraduationYear { get; }
    }
}