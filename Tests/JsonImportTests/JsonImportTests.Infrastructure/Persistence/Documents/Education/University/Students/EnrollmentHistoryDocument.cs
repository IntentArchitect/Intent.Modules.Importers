using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.Students;
using JsonImportTests.Domain.Repositories.Documents.Education.University.Students;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.Students
{
    internal class EnrollmentHistoryDocument : IEnrollmentHistoryDocument
    {
        public string Id { get; set; } = default!;
        public Guid SemesterId { get; set; }
        public string Semester { get; set; } = default!;
        public decimal Year { get; set; }
        public string EnrollmentStatus { get; set; } = default!;
        public decimal CreditHours { get; set; }
        public decimal SemesterGPA { get; set; }
        public bool DeansList { get; set; }
        public bool Probation { get; set; }
        public List<EnrollmentHistoryCourseDocument> Courses { get; set; } = default!;
        IReadOnlyList<IEnrollmentHistoryCourseDocument> IEnrollmentHistoryDocument.Courses => Courses;

        public EnrollmentHistory ToEntity(EnrollmentHistory? entity = default)
        {
            entity ??= new EnrollmentHistory();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.SemesterId = SemesterId;
            entity.Semester = Semester ?? throw new Exception($"{nameof(entity.Semester)} is null");
            entity.Year = Year;
            entity.EnrollmentStatus = EnrollmentStatus ?? throw new Exception($"{nameof(entity.EnrollmentStatus)} is null");
            entity.CreditHours = CreditHours;
            entity.SemesterGPA = SemesterGPA;
            entity.DeansList = DeansList;
            entity.Probation = Probation;
            entity.Courses = Courses.Select(x => x.ToEntity()).ToList();

            return entity;
        }

        public EnrollmentHistoryDocument PopulateFromEntity(EnrollmentHistory entity)
        {
            Id = entity.Id;
            SemesterId = entity.SemesterId;
            Semester = entity.Semester;
            Year = entity.Year;
            EnrollmentStatus = entity.EnrollmentStatus;
            CreditHours = entity.CreditHours;
            SemesterGPA = entity.SemesterGPA;
            DeansList = entity.DeansList;
            Probation = entity.Probation;
            Courses = entity.Courses.Select(x => EnrollmentHistoryCourseDocument.FromEntity(x)!).ToList();

            return this;
        }

        public static EnrollmentHistoryDocument? FromEntity(EnrollmentHistory? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new EnrollmentHistoryDocument().PopulateFromEntity(entity);
        }
    }
}