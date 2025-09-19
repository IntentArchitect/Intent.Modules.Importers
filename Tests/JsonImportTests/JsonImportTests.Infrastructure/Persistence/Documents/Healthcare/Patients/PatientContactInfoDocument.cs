using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Healthcare.Patients;
using JsonImportTests.Domain.Repositories.Documents.Healthcare.Patients;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Healthcare.Patients
{
    internal class PatientContactInfoDocument : IPatientContactInfoDocument
    {
        public string PrimaryPhone { get; set; } = default!;
        public string SecondaryPhone { get; set; } = default!;
        public string Email { get; set; } = default!;
        public PatientContactInfoAddressDocument Address { get; set; } = default!;
        IPatientContactInfoAddressDocument IPatientContactInfoDocument.Address => Address;

        public PatientContactInfo ToEntity(PatientContactInfo? entity = default)
        {
            entity ??= new PatientContactInfo();

            entity.PrimaryPhone = PrimaryPhone ?? throw new Exception($"{nameof(entity.PrimaryPhone)} is null");
            entity.SecondaryPhone = SecondaryPhone ?? throw new Exception($"{nameof(entity.SecondaryPhone)} is null");
            entity.Email = Email ?? throw new Exception($"{nameof(entity.Email)} is null");
            entity.Address = Address.ToEntity() ?? throw new Exception($"{nameof(entity.Address)} is null");

            return entity;
        }

        public PatientContactInfoDocument PopulateFromEntity(PatientContactInfo entity)
        {
            PrimaryPhone = entity.PrimaryPhone;
            SecondaryPhone = entity.SecondaryPhone;
            Email = entity.Email;
            Address = PatientContactInfoAddressDocument.FromEntity(entity.Address)!;

            return this;
        }

        public static PatientContactInfoDocument? FromEntity(PatientContactInfo? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new PatientContactInfoDocument().PopulateFromEntity(entity);
        }
    }
}