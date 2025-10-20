using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonImportTests.Application
{
    public class AddressDto
    {
        public AddressDto()
        {
            Street = null!;
            City = null!;
            State = null!;
            ZipCode = null!;
            Country = null!;
        }

        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }

        public static AddressDto Create(string street, string city, string state, string zipCode, string country)
        {
            return new AddressDto
            {
                Street = street,
                City = city,
                State = state,
                ZipCode = zipCode,
                Country = country
            };
        }
    }
}