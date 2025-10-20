using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonImportTests.Application.Education.Academic
{
    public class StudentDto
    {
        public StudentDto()
        {
            FirstName = null!;
            LastName = null!;
            Email = null!;
            StudentNumber = null!;
        }

        public Guid StudentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string StudentNumber { get; set; }

        public static StudentDto Create(Guid studentId, string firstName, string lastName, string email, string studentNumber)
        {
            return new StudentDto
            {
                StudentId = studentId,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                StudentNumber = studentNumber
            };
        }
    }
}