using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonImportTests.Application.Healthcare.Patients
{
    public class EmergencyContactDto
    {
        public EmergencyContactDto()
        {
            Name = null!;
            Relationship = null!;
            PhoneNumber = null!;
        }

        public string Name { get; set; }
        public string Relationship { get; set; }
        public string PhoneNumber { get; set; }

        public static EmergencyContactDto Create(string name, string relationship, string phoneNumber)
        {
            return new EmergencyContactDto
            {
                Name = name,
                Relationship = relationship,
                PhoneNumber = phoneNumber
            };
        }
    }
}