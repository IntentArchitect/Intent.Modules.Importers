using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Education.University.Academic
{
    public class Assignment
    {
        private Guid? _assignmentId;

        public Assignment()
        {
            Title = null!;
            Type = null!;
            Instructions = null!;
        }

        public Guid AssignmentId
        {
            get => _assignmentId ??= Guid.NewGuid();
            set => _assignmentId = value;
        }

        public string Title { get; set; }

        public string Type { get; set; }

        public DateTime DueDate { get; set; }

        public decimal Points { get; set; }

        public string Instructions { get; set; }
    }
}