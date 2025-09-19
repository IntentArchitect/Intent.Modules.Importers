using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.EdgeCases.ComplexTypes
{
    public class BasicInfo
    {
        public BasicInfo()
        {
            Name = null!;
            Email = null!;
        }

        public string Name { get; set; }

        public string Email { get; set; }
    }
}