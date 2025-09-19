using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.EdgeCases.ComplexTypes
{
    public interface IPrivacyDocument
    {
        string ProfileVisibility { get; }
        bool ShowEmail { get; }
        object ShowPhone { get; }
        bool AllowSearch { get; }
    }
}