using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.Students;
using JsonImportTests.Domain.Repositories.Documents.Education.University.Students;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.Students
{
    internal class HealthAndServicesInsuranceInfoDocument : IHealthAndServicesInsuranceInfoDocument
    {
        public bool HasInsurance { get; set; }
        public string InsuranceProvider { get; set; } = default!;
        public string PolicyNumber { get; set; } = default!;
        public string CoverageType { get; set; } = default!;

        public HealthAndServicesInsuranceInfo ToEntity(HealthAndServicesInsuranceInfo? entity = default)
        {
            entity ??= new HealthAndServicesInsuranceInfo();

            entity.HasInsurance = HasInsurance;
            entity.InsuranceProvider = InsuranceProvider ?? throw new Exception($"{nameof(entity.InsuranceProvider)} is null");
            entity.PolicyNumber = PolicyNumber ?? throw new Exception($"{nameof(entity.PolicyNumber)} is null");
            entity.CoverageType = CoverageType ?? throw new Exception($"{nameof(entity.CoverageType)} is null");

            return entity;
        }

        public HealthAndServicesInsuranceInfoDocument PopulateFromEntity(HealthAndServicesInsuranceInfo entity)
        {
            HasInsurance = entity.HasInsurance;
            InsuranceProvider = entity.InsuranceProvider;
            PolicyNumber = entity.PolicyNumber;
            CoverageType = entity.CoverageType;

            return this;
        }

        public static HealthAndServicesInsuranceInfoDocument? FromEntity(HealthAndServicesInsuranceInfo? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new HealthAndServicesInsuranceInfoDocument().PopulateFromEntity(entity);
        }
    }
}