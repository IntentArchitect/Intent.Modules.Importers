using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonImportTests.Application.Healthcare.Patients
{
    public class PractitionerDto
    {
        public PractitionerDto()
        {
            Name = null!;
            Specialization = null!;
            LicenseNumber = null!;
        }

        public Guid PractitionerId { get; set; }
        public string Name { get; set; }
        public string Specialization { get; set; }
        public string LicenseNumber { get; set; }

        public static PractitionerDto Create(Guid practitionerId, string name, string specialization, string licenseNumber)
        {
            return new PractitionerDto
            {
                PractitionerId = practitionerId,
                Name = name,
                Specialization = specialization,
                LicenseNumber = licenseNumber
            };
        }
    }
}