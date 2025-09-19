using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Education.University.Academic
{
    public class Department
    {
        public Department()
        {
            Name = null!;
            Code = null!;
            Faculty = null!;
        }

        public Guid DepartmentId { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public string Faculty { get; set; }
    }
}