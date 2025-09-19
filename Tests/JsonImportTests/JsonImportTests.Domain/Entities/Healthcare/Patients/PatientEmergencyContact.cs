using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Healthcare.Patients
{
    public class PatientEmergencyContact
    {
        private string? _id;

        public PatientEmergencyContact()
        {
            Id = null!;
            Name = null!;
            Relationship = null!;
            Phone = null!;
            Email = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public string Name { get; set; }

        public string Relationship { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }
    }
}