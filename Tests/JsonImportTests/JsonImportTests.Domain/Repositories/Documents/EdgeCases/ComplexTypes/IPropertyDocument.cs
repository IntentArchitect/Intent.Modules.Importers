using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.EdgeCases.ComplexTypes
{
    public interface IPropertyDocument
    {
        string Make { get; }
        string Model { get; }
        decimal Year { get; }
        string VIN { get; }
    }
}