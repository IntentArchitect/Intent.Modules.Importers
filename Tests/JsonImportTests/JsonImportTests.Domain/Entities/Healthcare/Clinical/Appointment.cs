using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Healthcare.Clinical
{
    public class Appointment
    {
        private Guid? _id;

        public Appointment()
        {
            AppointmentNumber = null!;
            AppointmentType = null!;
            Status = null!;
            Priority = null!;
            ReasonForVisit = null!;
            ChiefComplaint = null!;
            Notes = null!;
            VisitDetails = null!;
            Location = null!;
        }

        public Guid Id
        {
            get => _id ??= Guid.NewGuid();
            set => _id = value;
        }

        public string AppointmentNumber { get; set; }

        public Guid PatientId { get; set; }

        public Guid PractitionerId { get; set; }

        public string AppointmentType { get; set; }

        public string Status { get; set; }

        public string Priority { get; set; }

        public DateTime ScheduledDateTime { get; set; }

        public decimal EstimatedDuration { get; set; }

        public DateTime ActualStartTime { get; set; }

        public DateTime ActualEndTime { get; set; }

        public string ReasonForVisit { get; set; }

        public string ChiefComplaint { get; set; }

        public string Notes { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime LastModified { get; set; }

        public VisitDetail VisitDetails { get; set; }

        public AppointmentLocation Location { get; set; }
    }
}