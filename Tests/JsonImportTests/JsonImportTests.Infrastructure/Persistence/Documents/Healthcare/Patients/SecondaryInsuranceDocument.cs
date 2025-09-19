using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Healthcare.Patients;
using JsonImportTests.Domain.Repositories.Documents.Healthcare.Patients;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Healthcare.Patients
{
    internal class SecondaryInsuranceDocument : ISecondaryInsuranceDocument
    {
        public string ProviderName { get; set; } = default!;
        public string PolicyNumber { get; set; } = default!;
        public string GroupNumber { get; set; } = default!;
        public DateTime EffectiveDate { get; set; }
        public DateTime ExpirationDate { get; set; }

        public SecondaryInsurance ToEntity(SecondaryInsurance? entity = default)
        {
            entity ??= new SecondaryInsurance();

            entity.ProviderName = ProviderName ?? throw new Exception($"{nameof(entity.ProviderName)} is null");
            entity.PolicyNumber = PolicyNumber ?? throw new Exception($"{nameof(entity.PolicyNumber)} is null");
            entity.GroupNumber = GroupNumber ?? throw new Exception($"{nameof(entity.GroupNumber)} is null");
            entity.EffectiveDate = EffectiveDate;
            entity.ExpirationDate = ExpirationDate;

            return entity;
        }

        public SecondaryInsuranceDocument PopulateFromEntity(SecondaryInsurance entity)
        {
            ProviderName = entity.ProviderName;
            PolicyNumber = entity.PolicyNumber;
            GroupNumber = entity.GroupNumber;
            EffectiveDate = entity.EffectiveDate;
            ExpirationDate = entity.ExpirationDate;

            return this;
        }

        public static SecondaryInsuranceDocument? FromEntity(SecondaryInsurance? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new SecondaryInsuranceDocument().PopulateFromEntity(entity);
        }
    }
}