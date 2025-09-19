using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Healthcare.Patients;
using JsonImportTests.Domain.Repositories.Documents.Healthcare.Patients;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Healthcare.Patients
{
    internal class PatientInsuranceInfoDocument : IPatientInsuranceInfoDocument
    {
        public SecondaryInsuranceDocument SecondaryInsurance { get; set; } = default!;
        ISecondaryInsuranceDocument IPatientInsuranceInfoDocument.SecondaryInsurance => SecondaryInsurance;
        public PrimaryInsuranceDocument PrimaryInsurance { get; set; } = default!;
        IPrimaryInsuranceDocument IPatientInsuranceInfoDocument.PrimaryInsurance => PrimaryInsurance;

        public PatientInsuranceInfo ToEntity(PatientInsuranceInfo? entity = default)
        {
            entity ??= new PatientInsuranceInfo();
            entity.SecondaryInsurance = SecondaryInsurance.ToEntity() ?? throw new Exception($"{nameof(entity.SecondaryInsurance)} is null");
            entity.PrimaryInsurance = PrimaryInsurance.ToEntity() ?? throw new Exception($"{nameof(entity.PrimaryInsurance)} is null");

            return entity;
        }

        public PatientInsuranceInfoDocument PopulateFromEntity(PatientInsuranceInfo entity)
        {
            SecondaryInsurance = SecondaryInsuranceDocument.FromEntity(entity.SecondaryInsurance)!;
            PrimaryInsurance = PrimaryInsuranceDocument.FromEntity(entity.PrimaryInsurance)!;

            return this;
        }

        public static PatientInsuranceInfoDocument? FromEntity(PatientInsuranceInfo? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new PatientInsuranceInfoDocument().PopulateFromEntity(entity);
        }
    }
}