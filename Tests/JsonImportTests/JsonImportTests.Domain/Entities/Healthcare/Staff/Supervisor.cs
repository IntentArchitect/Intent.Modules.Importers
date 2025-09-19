using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Healthcare.Staff
{
    public class Supervisor
    {
        public Supervisor()
        {
            Name = null!;
            Title = null!;
        }

        public Guid SupervisorId { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }
    }
}