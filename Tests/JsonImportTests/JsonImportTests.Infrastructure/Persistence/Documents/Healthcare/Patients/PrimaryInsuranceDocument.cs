using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Healthcare.Patients;
using JsonImportTests.Domain.Repositories.Documents.Healthcare.Patients;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Healthcare.Patients
{
    internal class PrimaryInsuranceDocument : IPrimaryInsuranceDocument
    {
        public string ProviderName { get; set; } = default!;
        public string PolicyNumber { get; set; } = default!;
        public string GroupNumber { get; set; } = default!;
        public DateTime EffectiveDate { get; set; }
        public DateTime ExpirationDate { get; set; }

        public PrimaryInsurance ToEntity(PrimaryInsurance? entity = default)
        {
            entity ??= new PrimaryInsurance();

            entity.ProviderName = ProviderName ?? throw new Exception($"{nameof(entity.ProviderName)} is null");
            entity.PolicyNumber = PolicyNumber ?? throw new Exception($"{nameof(entity.PolicyNumber)} is null");
            entity.GroupNumber = GroupNumber ?? throw new Exception($"{nameof(entity.GroupNumber)} is null");
            entity.EffectiveDate = EffectiveDate;
            entity.ExpirationDate = ExpirationDate;

            return entity;
        }

        public PrimaryInsuranceDocument PopulateFromEntity(PrimaryInsurance entity)
        {
            ProviderName = entity.ProviderName;
            PolicyNumber = entity.PolicyNumber;
            GroupNumber = entity.GroupNumber;
            EffectiveDate = entity.EffectiveDate;
            ExpirationDate = entity.ExpirationDate;

            return this;
        }

        public static PrimaryInsuranceDocument? FromEntity(PrimaryInsurance? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new PrimaryInsuranceDocument().PopulateFromEntity(entity);
        }
    }
}