using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Education.University.Students
{
    public class GPA
    {
        public decimal CumulativeGPA { get; set; }

        public decimal SemesterGPA { get; set; }

        public decimal MajorGPA { get; set; }

        public decimal TotalCreditHours { get; set; }

        public decimal QualityPoints { get; set; }

        public decimal ClassRank { get; set; }

        public decimal ClassSize { get; set; }
    }
}