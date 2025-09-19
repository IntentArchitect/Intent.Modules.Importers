using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.EdgeCases.ComplexTypes
{
    public class BillingInfo
    {
        public BillingInfo()
        {
            Plan = null!;
            Currency = null!;
            PaymentMethod = null!;
        }

        public string Plan { get; set; }

        public decimal Amount { get; set; }

        public string Currency { get; set; }

        public DateTime NextBillingDate { get; set; }

        public PaymentMethod PaymentMethod { get; set; }
    }
}