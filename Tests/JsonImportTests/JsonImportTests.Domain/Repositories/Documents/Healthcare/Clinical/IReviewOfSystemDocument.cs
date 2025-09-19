using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Healthcare.Clinical
{
    public interface IReviewOfSystemDocument
    {
        string Constitutional { get; }
        string Cardiovascular { get; }
        string Respiratory { get; }
        string Gastrointestinal { get; }
        string Neurological { get; }
        string Musculoskeletal { get; }
    }
}