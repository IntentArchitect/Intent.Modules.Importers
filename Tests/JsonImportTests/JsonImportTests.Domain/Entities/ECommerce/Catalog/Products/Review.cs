using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.ECommerce.Catalog.Products
{
    public class Review
    {
        public Review()
        {
            RatingDistribution = null!;
        }

        public decimal AverageRating { get; set; }

        public decimal TotalReviews { get; set; }

        public RatingDistribution RatingDistribution { get; set; }
    }
}