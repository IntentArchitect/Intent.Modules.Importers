using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Healthcare.Patients;
using JsonImportTests.Domain.Repositories.Documents.Healthcare.Patients;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Healthcare.Patients
{
    internal class PatientEmergencyContactDocument : IPatientEmergencyContactDocument
    {
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Relationship { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public string Email { get; set; } = default!;

        public PatientEmergencyContact ToEntity(PatientEmergencyContact? entity = default)
        {
            entity ??= new PatientEmergencyContact();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");
            entity.Relationship = Relationship ?? throw new Exception($"{nameof(entity.Relationship)} is null");
            entity.Phone = Phone ?? throw new Exception($"{nameof(entity.Phone)} is null");
            entity.Email = Email ?? throw new Exception($"{nameof(entity.Email)} is null");

            return entity;
        }

        public PatientEmergencyContactDocument PopulateFromEntity(PatientEmergencyContact entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            Relationship = entity.Relationship;
            Phone = entity.Phone;
            Email = entity.Email;

            return this;
        }

        public static PatientEmergencyContactDocument? FromEntity(PatientEmergencyContact? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new PatientEmergencyContactDocument().PopulateFromEntity(entity);
        }
    }
}