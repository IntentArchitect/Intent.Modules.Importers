using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonImportTests.Application
{
    public class ProfileDto
    {
        public ProfileDto()
        {
            DisplayName = null!;
            Bio = null!;
            AvatarUrl = null!;
        }

        public string DisplayName { get; set; }
        public string Bio { get; set; }
        public string AvatarUrl { get; set; }

        public static ProfileDto Create(string displayName, string bio, string avatarUrl)
        {
            return new ProfileDto
            {
                DisplayName = displayName,
                Bio = bio,
                AvatarUrl = avatarUrl
            };
        }
    }
}