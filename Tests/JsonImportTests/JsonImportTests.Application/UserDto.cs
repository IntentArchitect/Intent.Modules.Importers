using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonImportTests.Application
{
    public class UserDto
    {
        public UserDto()
        {
            FirstName = null!;
            LastName = null!;
            Email = null!;
            Address = null!;
            PhoneNumbers = null!;
        }

        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public bool IsActive { get; set; }
        public AddressDto Address { get; set; }
        public List<PhoneNumberDto> PhoneNumbers { get; set; }

        public static UserDto Create(
            Guid id,
            string firstName,
            string lastName,
            string email,
            DateTime dateOfBirth,
            bool isActive,
            AddressDto address,
            List<PhoneNumberDto> phoneNumbers)
        {
            return new UserDto
            {
                Id = id,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                DateOfBirth = dateOfBirth,
                IsActive = isActive,
                Address = address,
                PhoneNumbers = phoneNumbers
            };
        }
    }
}