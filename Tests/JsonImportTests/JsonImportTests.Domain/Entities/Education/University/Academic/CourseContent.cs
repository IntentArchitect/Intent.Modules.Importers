using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Education.University.Academic
{
    public class CourseContent
    {
        public ICollection<TextBook> TextBooks { get; set; } = [];

        public ICollection<OnlineResource> OnlineResources { get; set; } = [];

        public ICollection<Module> Modules { get; set; } = [];
    }
}