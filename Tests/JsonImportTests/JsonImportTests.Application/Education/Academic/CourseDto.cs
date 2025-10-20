using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonImportTests.Application.Education.Academic
{
    public class CourseDto
    {
        public CourseDto()
        {
            CourseCode = null!;
            CourseName = null!;
            Instructor = null!;
        }

        public Guid CourseId { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public decimal Credits { get; set; }
        public InstructorDto Instructor { get; set; }

        public static CourseDto Create(
            Guid courseId,
            string courseCode,
            string courseName,
            decimal credits,
            InstructorDto instructor)
        {
            return new CourseDto
            {
                CourseId = courseId,
                CourseCode = courseCode,
                CourseName = courseName,
                Credits = credits,
                Instructor = instructor
            };
        }
    }
}