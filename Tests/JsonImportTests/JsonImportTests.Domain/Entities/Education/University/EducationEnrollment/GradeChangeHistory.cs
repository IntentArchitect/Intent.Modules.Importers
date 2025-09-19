using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Education.University.EducationEnrollment
{
    public class GradeChangeHistory
    {
        private string? _id;

        public GradeChangeHistory()
        {
            Id = null!;
            OldGrade = null!;
            NewGrade = null!;
            ChangeReason = null!;
            ChangedBy = null!;
            ApprovedBy = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public Guid ChangeId { get; set; }

        public string OldGrade { get; set; }

        public string NewGrade { get; set; }

        public string ChangeReason { get; set; }

        public string ChangedBy { get; set; }

        public DateTime ChangeDate { get; set; }

        public string ApprovedBy { get; set; }
    }
}