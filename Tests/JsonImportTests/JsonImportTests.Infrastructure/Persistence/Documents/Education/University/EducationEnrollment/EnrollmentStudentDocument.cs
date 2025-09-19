using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.EducationEnrollment;
using JsonImportTests.Domain.Repositories.Documents.Education.University.EducationEnrollment;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.EducationEnrollment
{
    internal class EnrollmentStudentDocument : IEnrollmentStudentDocument
    {
        public Guid StudentId { get; set; }
        public string StudentNumber { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Program { get; set; } = default!;
        public string ClassLevel { get; set; } = default!;

        public EnrollmentStudent ToEntity(EnrollmentStudent? entity = default)
        {
            entity ??= new EnrollmentStudent();

            entity.StudentId = StudentId;
            entity.StudentNumber = StudentNumber ?? throw new Exception($"{nameof(entity.StudentNumber)} is null");
            entity.FirstName = FirstName ?? throw new Exception($"{nameof(entity.FirstName)} is null");
            entity.LastName = LastName ?? throw new Exception($"{nameof(entity.LastName)} is null");
            entity.Email = Email ?? throw new Exception($"{nameof(entity.Email)} is null");
            entity.Program = Program ?? throw new Exception($"{nameof(entity.Program)} is null");
            entity.ClassLevel = ClassLevel ?? throw new Exception($"{nameof(entity.ClassLevel)} is null");

            return entity;
        }

        public EnrollmentStudentDocument PopulateFromEntity(EnrollmentStudent entity)
        {
            StudentId = entity.StudentId;
            StudentNumber = entity.StudentNumber;
            FirstName = entity.FirstName;
            LastName = entity.LastName;
            Email = entity.Email;
            Program = entity.Program;
            ClassLevel = entity.ClassLevel;

            return this;
        }

        public static EnrollmentStudentDocument? FromEntity(EnrollmentStudent? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new EnrollmentStudentDocument().PopulateFromEntity(entity);
        }
    }
}