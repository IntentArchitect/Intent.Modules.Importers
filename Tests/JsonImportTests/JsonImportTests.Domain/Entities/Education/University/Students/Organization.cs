using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Education.University.Students
{
    public class Organization
    {
        private Guid? _organizationId;

        public Organization()
        {
            Name = null!;
            Role = null!;
        }

        public Guid OrganizationId
        {
            get => _organizationId ??= Guid.NewGuid();
            set => _organizationId = value;
        }

        public string Name { get; set; }

        public string Role { get; set; }

        public DateTime JoinDate { get; set; }

        public bool IsActive { get; set; }
    }
}