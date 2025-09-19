using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.ECommerce.Catalog.Products
{
    public class RatingDistribution
    {
        public decimal _5Star { get; set; }

        public decimal _4Star { get; set; }

        public decimal _3Star { get; set; }

        public decimal _2Star { get; set; }

        public decimal _1Star { get; set; }
    }
}