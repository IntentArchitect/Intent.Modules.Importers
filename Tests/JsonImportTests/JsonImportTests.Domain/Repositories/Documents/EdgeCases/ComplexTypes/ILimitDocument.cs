using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.EdgeCases.ComplexTypes
{
    public interface ILimitDocument
    {
        decimal APICallsPerDay { get; }
        decimal StorageGB { get; }
        decimal ConcurrentConnections { get; }
    }
}