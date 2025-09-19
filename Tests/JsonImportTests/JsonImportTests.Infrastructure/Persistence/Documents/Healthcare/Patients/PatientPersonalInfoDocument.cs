using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Healthcare.Patients;
using JsonImportTests.Domain.Repositories.Documents.Healthcare.Patients;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Healthcare.Patients
{
    internal class PatientPersonalInfoDocument : IPatientPersonalInfoDocument
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = default!;
        public string SocialSecurityNumber { get; set; } = default!;

        public PatientPersonalInfo ToEntity(PatientPersonalInfo? entity = default)
        {
            entity ??= new PatientPersonalInfo();

            entity.FirstName = FirstName ?? throw new Exception($"{nameof(entity.FirstName)} is null");
            entity.LastName = LastName ?? throw new Exception($"{nameof(entity.LastName)} is null");
            entity.DateOfBirth = DateOfBirth;
            entity.Gender = Gender ?? throw new Exception($"{nameof(entity.Gender)} is null");
            entity.SocialSecurityNumber = SocialSecurityNumber ?? throw new Exception($"{nameof(entity.SocialSecurityNumber)} is null");

            return entity;
        }

        public PatientPersonalInfoDocument PopulateFromEntity(PatientPersonalInfo entity)
        {
            FirstName = entity.FirstName;
            LastName = entity.LastName;
            DateOfBirth = entity.DateOfBirth;
            Gender = entity.Gender;
            SocialSecurityNumber = entity.SocialSecurityNumber;

            return this;
        }

        public static PatientPersonalInfoDocument? FromEntity(PatientPersonalInfo? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new PatientPersonalInfoDocument().PopulateFromEntity(entity);
        }
    }
}