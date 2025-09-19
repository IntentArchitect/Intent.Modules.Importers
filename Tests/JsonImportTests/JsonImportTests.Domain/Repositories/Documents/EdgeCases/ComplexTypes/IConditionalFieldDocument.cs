using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.EdgeCases.ComplexTypes
{
    public interface IConditionalFieldDocument
    {
        string UserType { get; }
        object AdminInfo { get; }
        object GuestInfo { get; }
        IPremiumInfoDocument PremiumInfo { get; }
        IBasicInfoDocument BasicInfo { get; }
    }
}