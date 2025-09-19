using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.ECommerce.Catalog.Products
{
    public class DiscountRule
    {
        private string? _id;

        public DiscountRule()
        {
            Id = null!;
            Type = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public Guid RuleId { get; set; }

        public string Type { get; set; }

        public decimal Value { get; set; }

        public decimal MinQuantity { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}