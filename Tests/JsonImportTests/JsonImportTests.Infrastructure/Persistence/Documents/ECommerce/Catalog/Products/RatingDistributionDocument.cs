using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.ECommerce.Catalog.Products;
using JsonImportTests.Domain.Repositories.Documents.ECommerce.Catalog.Products;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.ECommerce.Catalog.Products
{
    internal class RatingDistributionDocument : IRatingDistributionDocument
    {
        public decimal Star5 { get; set; }
        public decimal Star4 { get; set; }
        public decimal Star3 { get; set; }
        public decimal Star2 { get; set; }
        public decimal Star1 { get; set; }

        public RatingDistribution ToEntity(RatingDistribution? entity = default)
        {
            entity ??= new RatingDistribution();

            entity.Star5 = Star5;
            entity.Star4 = Star4;
            entity.Star3 = Star3;
            entity.Star2 = Star2;
            entity.Star1 = Star1;

            return entity;
        }

        public RatingDistributionDocument PopulateFromEntity(RatingDistribution entity)
        {
            Star5 = entity.Star5;
            Star4 = entity.Star4;
            Star3 = entity.Star3;
            Star2 = entity.Star2;
            Star1 = entity.Star1;

            return this;
        }

        public static RatingDistributionDocument? FromEntity(RatingDistribution? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new RatingDistributionDocument().PopulateFromEntity(entity);
        }
    }
}