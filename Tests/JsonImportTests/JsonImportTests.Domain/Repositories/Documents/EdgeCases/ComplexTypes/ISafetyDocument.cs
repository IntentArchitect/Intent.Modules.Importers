using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.EdgeCases.ComplexTypes
{
    public interface ISafetyDocument
    {
        decimal AirbagsCount { get; }
        bool HasABS { get; }
        bool HasESC { get; }
        decimal CrashRating { get; }
        IReadOnlyList<string> OptionalSafetyFeatures { get; }
    }
}