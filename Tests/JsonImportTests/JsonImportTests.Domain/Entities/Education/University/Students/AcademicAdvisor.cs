using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Education.University.Students
{
    public class AcademicAdvisor
    {
        public AcademicAdvisor()
        {
            Name = null!;
            Email = null!;
            Department = null!;
        }

        public Guid AdvisorId { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Department { get; set; }

        public DateTime AssignedDate { get; set; }
    }
}