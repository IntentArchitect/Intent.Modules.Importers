using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Education.University.Students
{
    public class Sport
    {
        private Guid? _sportId;

        public Sport()
        {
            SportName = null!;
            Position = null!;
            Season = null!;
        }

        public Guid SportId
        {
            get => _sportId ??= Guid.NewGuid();
            set => _sportId = value;
        }

        public string SportName { get; set; }

        public string Position { get; set; }

        public string Season { get; set; }

        public decimal Year { get; set; }

        public decimal ScholarshipAmount { get; set; }
    }
}