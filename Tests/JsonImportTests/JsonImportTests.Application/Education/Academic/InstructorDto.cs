using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonImportTests.Application.Education.Academic
{
    public class InstructorDto
    {
        public InstructorDto()
        {
            Name = null!;
            Email = null!;
        }

        public Guid InstructorId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public static InstructorDto Create(Guid instructorId, string name, string email)
        {
            return new InstructorDto
            {
                InstructorId = instructorId,
                Name = name,
                Email = email
            };
        }
    }
}