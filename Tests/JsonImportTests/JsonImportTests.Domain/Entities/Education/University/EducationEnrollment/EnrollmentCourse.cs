using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Education.University.EducationEnrollment
{
    public class EnrollmentCourse
    {
        public EnrollmentCourse()
        {
            CourseCode = null!;
            CourseName = null!;
            Department = null!;
        }

        public Guid CourseId { get; set; }

        public string CourseCode { get; set; }

        public string CourseName { get; set; }

        public decimal CreditHours { get; set; }

        public string Department { get; set; }
    }
}