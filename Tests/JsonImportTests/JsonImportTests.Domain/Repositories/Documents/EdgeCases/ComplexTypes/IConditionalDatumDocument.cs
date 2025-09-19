using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.EdgeCases.ComplexTypes
{
    public interface IConditionalDatumDocument
    {
        string WhenPresent { get; }
        object MaybeNull { get; }
        IReadOnlyList<string> OptionalArray { get; }
    }
}