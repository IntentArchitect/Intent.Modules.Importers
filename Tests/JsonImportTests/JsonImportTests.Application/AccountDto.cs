using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonImportTests.Application
{
    public class AccountDto
    {
        public AccountDto()
        {
            Username = null!;
            Email = null!;
            Profile = null!;
        }

        public Guid AccountId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastLoginAt { get; set; }
        public bool IsVerified { get; set; }
        public ProfileDto Profile { get; set; }

        public static AccountDto Create(
            Guid accountId,
            string username,
            string email,
            DateTime createdAt,
            DateTime lastLoginAt,
            bool isVerified,
            ProfileDto profile)
        {
            return new AccountDto
            {
                AccountId = accountId,
                Username = username,
                Email = email,
                CreatedAt = createdAt,
                LastLoginAt = lastLoginAt,
                IsVerified = isVerified,
                Profile = profile
            };
        }
    }
}