using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.ECommerce.Catalog.Products
{
    public class RatingDistribution
    {
        public decimal Star5 { get; set; }

        public decimal Star4 { get; set; }

        public decimal Star3 { get; set; }

        public decimal Star2 { get; set; }

        public decimal Star1 { get; set; }
    }
}