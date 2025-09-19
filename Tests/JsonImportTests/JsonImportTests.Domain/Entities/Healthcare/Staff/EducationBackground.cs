using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Healthcare.Staff
{
    public class EducationBackground
    {
        private string? _id;

        public EducationBackground()
        {
            Id = null!;
            Institution = null!;
            Degree = null!;
            FieldOfStudy = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public string Institution { get; set; }

        public string Degree { get; set; }

        public string FieldOfStudy { get; set; }

        public decimal GraduationYear { get; set; }
    }
}