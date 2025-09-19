using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Education.University.Students
{
    public interface IActivityDocument
    {
        IReadOnlyList<ISportDocument> Sports { get; }
        IReadOnlyList<IOrganizationDocument> Organizations { get; }
        IReadOnlyList<IHonorDocument> Honors { get; }
    }
}