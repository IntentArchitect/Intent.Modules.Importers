using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Education.University.EducationEnrollment
{
    public class EnrollmentStudent
    {
        public EnrollmentStudent()
        {
            StudentNumber = null!;
            FirstName = null!;
            LastName = null!;
            Email = null!;
            Program = null!;
            ClassLevel = null!;
        }

        public Guid StudentId { get; set; }

        public string StudentNumber { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Program { get; set; }

        public string ClassLevel { get; set; }
    }
}