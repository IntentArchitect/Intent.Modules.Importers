using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.EdgeCases.ComplexTypes
{
    public class PremiumInfo
    {
        public PremiumInfo()
        {
            SubscriptionLevel = null!;
            Limits = null!;
            BillingInfo = null!;
        }

        public string SubscriptionLevel { get; set; }

        public IList<string> Features { get; set; } = [];

        public Limit Limits { get; set; }

        public BillingInfo BillingInfo { get; set; }
    }
}