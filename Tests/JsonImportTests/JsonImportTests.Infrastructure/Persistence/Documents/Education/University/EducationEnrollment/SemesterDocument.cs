using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.EducationEnrollment;
using JsonImportTests.Domain.Repositories.Documents.Education.University.EducationEnrollment;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.EducationEnrollment
{
    internal class SemesterDocument : ISemesterDocument
    {
        public Guid SemesterId { get; set; }
        public string SemesterName { get; set; } = default!;
        public decimal Year { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsCurrentSemester { get; set; }

        public Semester ToEntity(Semester? entity = default)
        {
            entity ??= new Semester();

            entity.SemesterId = SemesterId;
            entity.SemesterName = SemesterName ?? throw new Exception($"{nameof(entity.SemesterName)} is null");
            entity.Year = Year;
            entity.StartDate = StartDate;
            entity.EndDate = EndDate;
            entity.IsCurrentSemester = IsCurrentSemester;

            return entity;
        }

        public SemesterDocument PopulateFromEntity(Semester entity)
        {
            SemesterId = entity.SemesterId;
            SemesterName = entity.SemesterName;
            Year = entity.Year;
            StartDate = entity.StartDate;
            EndDate = entity.EndDate;
            IsCurrentSemester = entity.IsCurrentSemester;

            return this;
        }

        public static SemesterDocument? FromEntity(Semester? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new SemesterDocument().PopulateFromEntity(entity);
        }
    }
}