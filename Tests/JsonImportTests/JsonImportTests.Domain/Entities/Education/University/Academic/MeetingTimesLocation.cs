using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Education.University.Academic
{
    public class MeetingTimesLocation
    {
        public MeetingTimesLocation()
        {
            Building = null!;
            Room = null!;
        }

        public string Building { get; set; }

        public string Room { get; set; }

        public decimal Capacity { get; set; }

        public IList<string> Equipment { get; set; } = [];
    }
}