using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Education.University.Academic
{
    public class Module
    {
        private Guid? _moduleId;

        public Module()
        {
            Title = null!;
        }

        public Guid ModuleId
        {
            get => _moduleId ??= Guid.NewGuid();
            set => _moduleId = value;
        }

        public string Title { get; set; }

        public decimal Order { get; set; }

        public IList<string> LearningObjectives { get; set; } = [];

        public ICollection<Topic> Topics { get; set; } = [];

        public ICollection<Assignment> Assignments { get; set; } = [];
    }
}