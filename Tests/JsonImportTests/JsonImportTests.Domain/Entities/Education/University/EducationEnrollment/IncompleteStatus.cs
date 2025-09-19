using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Education.University.EducationEnrollment
{
    public class IncompleteStatus
    {
        public IncompleteStatus()
        {
            IncompleteReason = null!;
            CompletionDeadline = null!;
        }

        public bool IsIncomplete { get; set; }

        public object IncompleteReason { get; set; }

        public object CompletionDeadline { get; set; }

        public ICollection<object> ExtensionRequests { get; set; } = [];
    }
}