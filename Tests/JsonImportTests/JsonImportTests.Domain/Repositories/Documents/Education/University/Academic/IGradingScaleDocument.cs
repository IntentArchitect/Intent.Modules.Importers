using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Education.University.Academic
{
    public interface IGradingScaleDocument
    {
        string Id { get; }
        string Grade { get; }
        decimal MinPercentage { get; }
        decimal MaxPercentage { get; }
        decimal GradePoints { get; }
    }
}