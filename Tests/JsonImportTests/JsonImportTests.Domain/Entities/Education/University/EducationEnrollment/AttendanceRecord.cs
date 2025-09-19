using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Education.University.EducationEnrollment
{
    public class AttendanceRecord
    {
        private string? _id;

        public AttendanceRecord()
        {
            Id = null!;
            Status = null!;
            Notes = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public DateTime Date { get; set; }

        public string Status { get; set; }

        public string Notes { get; set; }
    }
}