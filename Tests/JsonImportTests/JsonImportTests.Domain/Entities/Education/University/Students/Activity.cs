using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Education.University.Students
{
    public class Activity
    {
        public ICollection<Sport> Sports { get; set; } = [];

        public ICollection<Organization> Organizations { get; set; } = [];

        public ICollection<Honor> Honors { get; set; } = [];
    }
}