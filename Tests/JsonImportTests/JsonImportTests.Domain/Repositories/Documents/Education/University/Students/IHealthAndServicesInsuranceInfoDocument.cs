using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Education.University.Students
{
    public interface IHealthAndServicesInsuranceInfoDocument
    {
        bool HasInsurance { get; }
        string InsuranceProvider { get; }
        string PolicyNumber { get; }
        string CoverageType { get; }
    }
}