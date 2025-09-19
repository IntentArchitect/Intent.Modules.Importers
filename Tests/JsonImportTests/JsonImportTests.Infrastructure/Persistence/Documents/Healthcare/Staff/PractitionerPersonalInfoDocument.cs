using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Healthcare.Staff;
using JsonImportTests.Domain.Repositories.Documents.Healthcare.Staff;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Healthcare.Staff
{
    internal class PractitionerPersonalInfoDocument : IPractitionerPersonalInfoDocument
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = default!;

        public PractitionerPersonalInfo ToEntity(PractitionerPersonalInfo? entity = default)
        {
            entity ??= new PractitionerPersonalInfo();

            entity.FirstName = FirstName ?? throw new Exception($"{nameof(entity.FirstName)} is null");
            entity.LastName = LastName ?? throw new Exception($"{nameof(entity.LastName)} is null");
            entity.DateOfBirth = DateOfBirth;
            entity.Gender = Gender ?? throw new Exception($"{nameof(entity.Gender)} is null");

            return entity;
        }

        public PractitionerPersonalInfoDocument PopulateFromEntity(PractitionerPersonalInfo entity)
        {
            FirstName = entity.FirstName;
            LastName = entity.LastName;
            DateOfBirth = entity.DateOfBirth;
            Gender = entity.Gender;

            return this;
        }

        public static PractitionerPersonalInfoDocument? FromEntity(PractitionerPersonalInfo? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new PractitionerPersonalInfoDocument().PopulateFromEntity(entity);
        }
    }
}