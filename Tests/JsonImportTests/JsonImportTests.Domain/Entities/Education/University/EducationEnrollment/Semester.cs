using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Education.University.EducationEnrollment
{
    public class Semester
    {
        public Semester()
        {
            SemesterName = null!;
        }

        public Guid SemesterId { get; set; }

        public string SemesterName { get; set; }

        public decimal Year { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsCurrentSemester { get; set; }
    }
}