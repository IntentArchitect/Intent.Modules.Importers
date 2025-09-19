using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Healthcare.Clinical
{
    public class CreatedBy
    {
        public CreatedBy()
        {
            Name = null!;
            Title = null!;
        }

        public Guid PractitionerId { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }
    }
}