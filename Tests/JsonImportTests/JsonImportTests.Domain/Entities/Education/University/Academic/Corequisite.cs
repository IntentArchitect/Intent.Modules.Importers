using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Education.University.Academic
{
    public class Corequisite
    {
        private string? _id;

        public Corequisite()
        {
            Id = null!;
            CourseCode = null!;
            CourseName = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public Guid CourseId { get; set; }

        public string CourseCode { get; set; }

        public string CourseName { get; set; }

        public bool CanBeTakenConcurrently { get; set; }
    }
}