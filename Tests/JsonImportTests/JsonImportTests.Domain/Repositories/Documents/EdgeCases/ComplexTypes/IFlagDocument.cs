using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.EdgeCases.ComplexTypes
{
    public interface IFlagDocument
    {
        bool IsActive { get; }
        bool IsDeleted { get; }
        bool IsPublic { get; }
        object IsVerified { get; }
        bool RequiresApproval { get; }
        bool HasNotifications { get; }
    }
}