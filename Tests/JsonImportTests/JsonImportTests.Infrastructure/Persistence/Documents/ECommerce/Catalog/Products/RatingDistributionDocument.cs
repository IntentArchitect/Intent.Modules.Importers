using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.ECommerce.Catalog.Products;
using JsonImportTests.Domain.Repositories.Documents.ECommerce.Catalog.Products;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.ECommerce.Catalog.Products
{
    internal class RatingDistributionDocument : IRatingDistributionDocument
    {
        public decimal 5Star { get; set; }
    public decimal 4Star { get; set; }
public decimal 3Star { get; set; }
        public decimal 2Star { get; set; }
        public decimal 1Star { get; set; }

        public RatingDistribution ToEntity(RatingDistribution? entity = default)
{
    entity ??= new RatingDistribution();

    entity.5Star = 5Star;
    entity.4Star = 4Star;
    entity.3Star = 3Star;
    entity.2Star = 2Star;
    entity.1Star = 1Star;

    return entity;
}

public RatingDistributionDocument PopulateFromEntity(RatingDistribution entity)
{
    5Star = entity.5Star;
    4Star = entity.4Star;
    3Star = entity.3Star;
    2Star = entity.2Star;
    1Star = entity.1Star;

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