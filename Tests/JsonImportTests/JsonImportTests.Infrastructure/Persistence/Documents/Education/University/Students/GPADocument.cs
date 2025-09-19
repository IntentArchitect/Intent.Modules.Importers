using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.Students;
using JsonImportTests.Domain.Repositories.Documents.Education.University.Students;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.Students
{
    internal class GPADocument : IGPADocument
    {
        public decimal CumulativeGPA { get; set; }
        public decimal SemesterGPA { get; set; }
        public decimal MajorGPA { get; set; }
        public decimal TotalCreditHours { get; set; }
        public decimal QualityPoints { get; set; }
        public decimal ClassRank { get; set; }
        public decimal ClassSize { get; set; }

        public GPA ToEntity(GPA? entity = default)
        {
            entity ??= new GPA();

            entity.CumulativeGPA = CumulativeGPA;
            entity.SemesterGPA = SemesterGPA;
            entity.MajorGPA = MajorGPA;
            entity.TotalCreditHours = TotalCreditHours;
            entity.QualityPoints = QualityPoints;
            entity.ClassRank = ClassRank;
            entity.ClassSize = ClassSize;

            return entity;
        }

        public GPADocument PopulateFromEntity(GPA entity)
        {
            CumulativeGPA = entity.CumulativeGPA;
            SemesterGPA = entity.SemesterGPA;
            MajorGPA = entity.MajorGPA;
            TotalCreditHours = entity.TotalCreditHours;
            QualityPoints = entity.QualityPoints;
            ClassRank = entity.ClassRank;
            ClassSize = entity.ClassSize;

            return this;
        }

        public static GPADocument? FromEntity(GPA? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new GPADocument().PopulateFromEntity(entity);
        }
    }
}