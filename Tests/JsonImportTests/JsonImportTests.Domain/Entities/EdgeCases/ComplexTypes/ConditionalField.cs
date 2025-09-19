using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.EdgeCases.ComplexTypes
{
    public class ConditionalField
    {
        public ConditionalField()
        {
            UserType = null!;
            AdminInfo = null!;
            GuestInfo = null!;
            PremiumInfo = null!;
            BasicInfo = null!;
        }

        public string UserType { get; set; }

        public object AdminInfo { get; set; }

        public object GuestInfo { get; set; }

        public PremiumInfo PremiumInfo { get; set; }

        public BasicInfo BasicInfo { get; set; }
    }
}