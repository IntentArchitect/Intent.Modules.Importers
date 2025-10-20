using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonImportTests.Application.ECommerce.Orders
{
    public class ShippingAddressDto
    {
        public ShippingAddressDto()
        {
            RecipientName = null!;
            Street = null!;
            City = null!;
            State = null!;
            ZipCode = null!;
            Country = null!;
        }

        public string RecipientName { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }

        public static ShippingAddressDto Create(
            string recipientName,
            string street,
            string city,
            string state,
            string zipCode,
            string country)
        {
            return new ShippingAddressDto
            {
                RecipientName = recipientName,
                Street = street,
                City = city,
                State = state,
                ZipCode = zipCode,
                Country = country
            };
        }
    }
}