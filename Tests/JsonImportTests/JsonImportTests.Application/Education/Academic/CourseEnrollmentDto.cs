using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonImportTests.Application.Education.Academic
{
    public class CourseEnrollmentDto
    {
        public CourseEnrollmentDto()
        {
            Status = null!;
            Student = null!;
            Course = null!;
            Grades = null!;
        }

        public Guid EnrollmentId { get; set; }
        public Guid StudentId { get; set; }
        public Guid CourseId { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public string Status { get; set; }
        public StudentDto Student { get; set; }
        public CourseDto Course { get; set; }
        public List<GradeDto> Grades { get; set; }

        public static CourseEnrollmentDto Create(
            Guid enrollmentId,
            Guid studentId,
            Guid courseId,
            DateTime enrollmentDate,
            string status,
            StudentDto student,
            CourseDto course,
            List<GradeDto> grades)
        {
            return new CourseEnrollmentDto
            {
                EnrollmentId = enrollmentId,
                StudentId = studentId,
                CourseId = courseId,
                EnrollmentDate = enrollmentDate,
                Status = status,
                Student = student,
                Course = course,
                Grades = grades
            };
        }
    }
}