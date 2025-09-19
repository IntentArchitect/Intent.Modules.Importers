using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.EdgeCases.ComplexTypes
{
    public interface IEngineDocument
    {
        string Type { get; }
        decimal Displacement { get; }
        decimal Horsepower { get; }
        bool Turbo { get; }
    }
}