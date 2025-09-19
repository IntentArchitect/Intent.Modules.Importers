using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Education.University.EducationEnrollment
{
    public class Waiver
    {
        private Guid? _waiverId;

        public Waiver()
        {
            PrerequisiteWaived = null!;
            WaiverReason = null!;
            ApprovedBy = null!;
        }

        public Guid WaiverId
        {
            get => _waiverId ??= Guid.NewGuid();
            set => _waiverId = value;
        }

        public string PrerequisiteWaived { get; set; }

        public string WaiverReason { get; set; }

        public string ApprovedBy { get; set; }

        public DateTime ApprovalDate { get; set; }
    }
}