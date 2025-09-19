using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Education.University.Students
{
    public interface IHealthAndServiceDocument
    {
        IHealthAndServicesInsuranceInfoDocument InsuranceInfo { get; }
        IReadOnlyList<IDisabilityDocument> Disabilities { get; }
        ICounselingServiceDocument CounselingServices { get; }
    }
}