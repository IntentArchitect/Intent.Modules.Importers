using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Healthcare.Patients
{
    public class Patient
    {
        private Guid? _id;

        public Patient()
        {
            PatientNumber = null!;
            PersonalInfo = null!;
            MedicalHistory = null!;
            InsuranceInfo = null!;
            ContactInfo = null!;
        }

        public Guid Id
        {
            get => _id ??= Guid.NewGuid();
            set => _id = value;
        }

        public string PatientNumber { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime LastUpdated { get; set; }

        public bool IsActive { get; set; }

        public PatientPersonalInfo PersonalInfo { get; set; }

        public MedicalHistory MedicalHistory { get; set; }

        public PatientInsuranceInfo InsuranceInfo { get; set; }

        public ICollection<PatientEmergencyContact> EmergencyContacts { get; set; } = [];

        public PatientContactInfo ContactInfo { get; set; }
    }
}