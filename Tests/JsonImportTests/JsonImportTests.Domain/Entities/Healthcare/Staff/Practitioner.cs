using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Healthcare.Staff
{
    public class Practitioner
    {
        private Guid? _id;

        public Practitioner()
        {
            EmployeeId = null!;
            WorkSchedule = null!;
            Supervisor = null!;
            ProfessionalInfo = null!;
            PersonalInfo = null!;
            ContactInfo = null!;
        }

        public Guid Id
        {
            get => _id ??= Guid.NewGuid();
            set => _id = value;
        }

        public string EmployeeId { get; set; }

        public DateTime HireDate { get; set; }

        public bool IsActive { get; set; }

        public WorkSchedule WorkSchedule { get; set; }

        public Supervisor Supervisor { get; set; }

        public ProfessionalInfo ProfessionalInfo { get; set; }

        public PractitionerPersonalInfo PersonalInfo { get; set; }

        public ICollection<EducationBackground> EducationBackground { get; set; } = [];

        public PractitionerContactInfo ContactInfo { get; set; }

        public ICollection<Certification> Certifications { get; set; } = [];
    }
}