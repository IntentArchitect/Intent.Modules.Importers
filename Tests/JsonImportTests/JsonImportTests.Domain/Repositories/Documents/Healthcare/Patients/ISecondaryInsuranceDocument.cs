using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Healthcare.Patients
{
    public interface ISecondaryInsuranceDocument
    {
        string ProviderName { get; }
        string PolicyNumber { get; }
        string GroupNumber { get; }
        DateTime EffectiveDate { get; }
        DateTime ExpirationDate { get; }
    }
}