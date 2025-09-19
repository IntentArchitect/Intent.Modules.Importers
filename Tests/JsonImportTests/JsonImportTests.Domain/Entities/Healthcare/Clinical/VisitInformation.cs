using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Healthcare.Clinical
{
    public class VisitInformation
    {
        public VisitInformation()
        {
            VisitType = null!;
        }

        public Guid AppointmentId { get; set; }

        public DateTime VisitDate { get; set; }

        public string VisitType { get; set; }

        public Guid FacilityId { get; set; }
    }
}