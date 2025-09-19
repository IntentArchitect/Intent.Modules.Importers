using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.Students;
using JsonImportTests.Domain.Repositories.Documents.Education.University.Students;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.Students
{
    internal class EnrollmentHistoryCourseDocument : IEnrollmentHistoryCourseDocument
    {
        public string Id { get; set; } = default!;
        public Guid CourseId { get; set; }
        public string CourseCode { get; set; } = default!;
        public decimal CreditHours { get; set; }
        public string Grade { get; set; } = default!;
        public decimal GradePoints { get; set; }
        public DateTime GradeDate { get; set; }

        public EnrollmentHistoryCourse ToEntity(EnrollmentHistoryCourse? entity = default)
        {
            entity ??= new EnrollmentHistoryCourse();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.CourseId = CourseId;
            entity.CourseCode = CourseCode ?? throw new Exception($"{nameof(entity.CourseCode)} is null");
            entity.CreditHours = CreditHours;
            entity.Grade = Grade ?? throw new Exception($"{nameof(entity.Grade)} is null");
            entity.GradePoints = GradePoints;
            entity.GradeDate = GradeDate;

            return entity;
        }

        public EnrollmentHistoryCourseDocument PopulateFromEntity(EnrollmentHistoryCourse entity)
        {
            Id = entity.Id;
            CourseId = entity.CourseId;
            CourseCode = entity.CourseCode;
            CreditHours = entity.CreditHours;
            Grade = entity.Grade;
            GradePoints = entity.GradePoints;
            GradeDate = entity.GradeDate;

            return this;
        }

        public static EnrollmentHistoryCourseDocument? FromEntity(EnrollmentHistoryCourse? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new EnrollmentHistoryCourseDocument().PopulateFromEntity(entity);
        }
    }
}