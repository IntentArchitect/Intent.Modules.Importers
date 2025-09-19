using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.ECommerce.Catalog.Products
{
    public interface IReviewDocument
    {
        decimal AverageRating { get; }
        decimal TotalReviews { get; }
        IRatingDistributionDocument RatingDistribution { get; }
    }
}