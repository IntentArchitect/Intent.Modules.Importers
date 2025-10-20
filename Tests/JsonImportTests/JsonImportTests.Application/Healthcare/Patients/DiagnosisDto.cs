using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonImportTests.Application.Healthcare.Patients
{
    public class DiagnosisDto
    {
        public DiagnosisDto()
        {
            Code = null!;
            Description = null!;
            Severity = null!;
        }

        public string Code { get; set; }
        public string Description { get; set; }
        public string Severity { get; set; }

        public static DiagnosisDto Create(string code, string description, string severity)
        {
            return new DiagnosisDto
            {
                Code = code,
                Description = description,
                Severity = severity
            };
        }
    }
}