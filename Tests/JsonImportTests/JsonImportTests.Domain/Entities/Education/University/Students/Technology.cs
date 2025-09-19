using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Education.University.Students
{
    public class Technology
    {
        public Technology()
        {
            UniversityEmail = null!;
        }

        public string UniversityEmail { get; set; }

        public bool StudentPortalAccess { get; set; }

        public bool LibraryAccess { get; set; }

        public ICollection<ITService> ITServices { get; set; } = [];
    }
}