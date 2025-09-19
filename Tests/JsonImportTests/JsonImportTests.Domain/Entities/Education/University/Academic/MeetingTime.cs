using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Education.University.Academic
{
    public class MeetingTime
    {
        private string? _id;

        public MeetingTime()
        {
            Id = null!;
            Day = null!;
            StartTime = null!;
            EndTime = null!;
            Location = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public string Day { get; set; }

        public string StartTime { get; set; }

        public string EndTime { get; set; }

        public MeetingTimesLocation Location { get; set; }
    }
}