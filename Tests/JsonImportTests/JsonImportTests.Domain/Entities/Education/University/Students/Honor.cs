using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Education.University.Students
{
    public class Honor
    {
        private Guid? _honorId;

        public Honor()
        {
            Title = null!;
            Organization = null!;
            Description = null!;
        }

        public Guid HonorId
        {
            get => _honorId ??= Guid.NewGuid();
            set => _honorId = value;
        }

        public string Title { get; set; }

        public string Organization { get; set; }

        public DateTime DateReceived { get; set; }

        public string Description { get; set; }
    }
}