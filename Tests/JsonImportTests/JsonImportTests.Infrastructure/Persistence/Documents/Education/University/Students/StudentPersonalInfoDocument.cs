using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.Students;
using JsonImportTests.Domain.Repositories.Documents.Education.University.Students;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.Students
{
    internal class StudentPersonalInfoDocument : IStudentPersonalInfoDocument
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string MiddleName { get; set; } = default!;
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = default!;
        public string Nationality { get; set; } = default!;
        public string CitizenshipStatus { get; set; } = default!;

        public StudentPersonalInfo ToEntity(StudentPersonalInfo? entity = default)
        {
            entity ??= new StudentPersonalInfo();

            entity.FirstName = FirstName ?? throw new Exception($"{nameof(entity.FirstName)} is null");
            entity.LastName = LastName ?? throw new Exception($"{nameof(entity.LastName)} is null");
            entity.MiddleName = MiddleName ?? throw new Exception($"{nameof(entity.MiddleName)} is null");
            entity.DateOfBirth = DateOfBirth;
            entity.Gender = Gender ?? throw new Exception($"{nameof(entity.Gender)} is null");
            entity.Nationality = Nationality ?? throw new Exception($"{nameof(entity.Nationality)} is null");
            entity.CitizenshipStatus = CitizenshipStatus ?? throw new Exception($"{nameof(entity.CitizenshipStatus)} is null");

            return entity;
        }

        public StudentPersonalInfoDocument PopulateFromEntity(StudentPersonalInfo entity)
        {
            FirstName = entity.FirstName;
            LastName = entity.LastName;
            MiddleName = entity.MiddleName;
            DateOfBirth = entity.DateOfBirth;
            Gender = entity.Gender;
            Nationality = entity.Nationality;
            CitizenshipStatus = entity.CitizenshipStatus;

            return this;
        }

        public static StudentPersonalInfoDocument? FromEntity(StudentPersonalInfo? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new StudentPersonalInfoDocument().PopulateFromEntity(entity);
        }
    }
}