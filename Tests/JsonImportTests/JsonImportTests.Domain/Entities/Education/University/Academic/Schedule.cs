using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Education.University.Academic
{
    public class Schedule
    {
        public Schedule()
        {
            ExamSchedule = null!;
        }

        public ICollection<MeetingTime> MeetingTimes { get; set; } = [];

        public ExamSchedule ExamSchedule { get; set; }
    }
}