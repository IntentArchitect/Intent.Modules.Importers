using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.ECommerce.Catalog.Products;
using JsonImportTests.Domain.Repositories.Documents.ECommerce.Catalog.Products;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.ECommerce.Catalog.Products
{
    internal class ReviewDocument : IReviewDocument
    {
        public decimal AverageRating { get; set; }
        public decimal TotalReviews { get; set; }
        public RatingDistributionDocument RatingDistribution { get; set; } = default!;
        IRatingDistributionDocument IReviewDocument.RatingDistribution => RatingDistribution;

        public Review ToEntity(Review? entity = default)
        {
            entity ??= new Review();

            entity.AverageRating = AverageRating;
            entity.TotalReviews = TotalReviews;
            entity.RatingDistribution = RatingDistribution.ToEntity() ?? throw new Exception($"{nameof(entity.RatingDistribution)} is null");

            return entity;
        }

        public ReviewDocument PopulateFromEntity(Review entity)
        {
            AverageRating = entity.AverageRating;
            TotalReviews = entity.TotalReviews;
            RatingDistribution = RatingDistributionDocument.FromEntity(entity.RatingDistribution)!;

            return this;
        }

        public static ReviewDocument? FromEntity(Review? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new ReviewDocument().PopulateFromEntity(entity);
        }
    }
}