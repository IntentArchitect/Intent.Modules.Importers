using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Healthcare.Clinical
{
    public class AppointmentLocation
    {
        public AppointmentLocation()
        {
            Building = null!;
            Floor = null!;
            RoomNumber = null!;
        }

        public string Building { get; set; }

        public string Floor { get; set; }

        public string RoomNumber { get; set; }

        public Guid FacilityId { get; set; }
    }
}