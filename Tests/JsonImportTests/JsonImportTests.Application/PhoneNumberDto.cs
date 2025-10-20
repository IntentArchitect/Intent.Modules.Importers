using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonImportTests.Application
{
    public class PhoneNumberDto
    {
        public PhoneNumberDto()
        {
            Type = null!;
            Number = null!;
        }

        public string Type { get; set; }
        public string Number { get; set; }

        public static PhoneNumberDto Create(string type, string number)
        {
            return new PhoneNumberDto
            {
                Type = type,
                Number = number
            };
        }
    }
}