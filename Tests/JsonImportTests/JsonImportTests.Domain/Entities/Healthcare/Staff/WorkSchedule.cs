using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Healthcare.Staff
{
    public class WorkSchedule
    {
        public WorkSchedule()
        {
            ShiftType = null!;
            StartTime = null!;
            EndTime = null!;
        }

        public string ShiftType { get; set; }

        public decimal WeeklyHours { get; set; }

        public string StartTime { get; set; }

        public string EndTime { get; set; }

        public IList<string> WorkDays { get; set; } = [];
    }
}