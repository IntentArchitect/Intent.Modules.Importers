using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonImportTests.Application.Healthcare.Patients
{
    public class PatientDto
    {
        public PatientDto()
        {
            FirstName = null!;
            LastName = null!;
            Gender = null!;
            ContactNumber = null!;
            EmergencyContact = null!;
        }

        public Guid PatientId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string ContactNumber { get; set; }
        public EmergencyContactDto EmergencyContact { get; set; }

        public static PatientDto Create(
            Guid patientId,
            string firstName,
            string lastName,
            DateTime dateOfBirth,
            string gender,
            string contactNumber,
            EmergencyContactDto emergencyContact)
        {
            return new PatientDto
            {
                PatientId = patientId,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                Gender = gender,
                ContactNumber = contactNumber,
                EmergencyContact = emergencyContact
            };
        }
    }
}