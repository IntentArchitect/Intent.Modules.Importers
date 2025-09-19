using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.EdgeCases.ComplexTypes
{
    public interface ISettingDocument
    {
        string Theme { get; }
        string Language { get; }
        string Timezone { get; }
        IPrivacyDocument Privacy { get; }
        INotificationDocument Notifications { get; }
    }
}