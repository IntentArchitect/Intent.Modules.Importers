using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Education.University.EducationEnrollment
{
    public class EnrollmentInstructor
    {
        public EnrollmentInstructor()
        {
            Name = null!;
            Email = null!;
            Title = null!;
            Department = null!;
        }

        public Guid InstructorId { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Title { get; set; }

        public string Department { get; set; }
    }
}