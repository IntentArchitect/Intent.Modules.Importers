using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.EdgeCases.ComplexTypes
{
    public class ComplexInheritance
    {
        public ComplexInheritance()
        {
            BaseType = null!;
            SpecificProperties = null!;
            Properties = null!;
        }

        public string BaseType { get; set; }

        public SpecificProperty SpecificProperties { get; set; }

        public Property Properties { get; set; }
    }
}