using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Education.University.EducationEnrollment
{
    public class Attendance
    {
        public bool RequiredAttendance { get; set; }

        public decimal TotalSessions { get; set; }

        public decimal AttendedSessions { get; set; }

        public decimal ExcusedAbsences { get; set; }

        public decimal UnexcusedAbsences { get; set; }

        public decimal AttendancePercentage { get; set; }

        public ICollection<AttendanceRecord> AttendanceRecords { get; set; } = [];
    }
}