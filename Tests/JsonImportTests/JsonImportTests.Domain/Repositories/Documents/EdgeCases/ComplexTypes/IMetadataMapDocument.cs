using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.EdgeCases.ComplexTypes
{
    public interface IMetadataMapDocument
    {
        string Key1 { get; }
        decimal Key2 { get; }
        bool Key3 { get; }
        object Key4 { get; }
        IKey5Document Key5 { get; }
    }
}