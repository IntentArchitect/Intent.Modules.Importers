using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.EducationEnrollment;
using JsonImportTests.Domain.Repositories.Documents.Education.University.EducationEnrollment;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.EducationEnrollment
{
    internal class EnrollmentCourseDocument : IEnrollmentCourseDocument
    {
        public Guid CourseId { get; set; }
        public string CourseCode { get; set; } = default!;
        public string CourseName { get; set; } = default!;
        public decimal CreditHours { get; set; }
        public string Department { get; set; } = default!;

        public EnrollmentCourse ToEntity(EnrollmentCourse? entity = default)
        {
            entity ??= new EnrollmentCourse();

            entity.CourseId = CourseId;
            entity.CourseCode = CourseCode ?? throw new Exception($"{nameof(entity.CourseCode)} is null");
            entity.CourseName = CourseName ?? throw new Exception($"{nameof(entity.CourseName)} is null");
            entity.CreditHours = CreditHours;
            entity.Department = Department ?? throw new Exception($"{nameof(entity.Department)} is null");

            return entity;
        }

        public EnrollmentCourseDocument PopulateFromEntity(EnrollmentCourse entity)
        {
            CourseId = entity.CourseId;
            CourseCode = entity.CourseCode;
            CourseName = entity.CourseName;
            CreditHours = entity.CreditHours;
            Department = entity.Department;

            return this;
        }

        public static EnrollmentCourseDocument? FromEntity(EnrollmentCourse? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new EnrollmentCourseDocument().PopulateFromEntity(entity);
        }
    }
}