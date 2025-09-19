using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.EdgeCases.ComplexTypes
{
    public class PaymentMethod
    {
        public PaymentMethod()
        {
            Type = null!;
            Last4 = null!;
        }

        public string Type { get; set; }

        public string Last4 { get; set; }

        public decimal ExpiryMonth { get; set; }

        public decimal ExpiryYear { get; set; }
    }
}