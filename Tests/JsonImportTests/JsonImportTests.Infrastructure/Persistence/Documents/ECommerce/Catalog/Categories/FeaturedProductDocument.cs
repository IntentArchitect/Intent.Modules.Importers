using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.ECommerce.Catalog.Categories;
using JsonImportTests.Domain.Repositories.Documents.ECommerce.Catalog.Categories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.ECommerce.Catalog.Categories
{
    internal class FeaturedProductDocument : IFeaturedProductDocument
    {
        public string Id { get; set; } = default!;
        public Guid ProductId { get; set; }
        public decimal Position { get; set; }
        public bool IsSponsored { get; set; }

        public FeaturedProduct ToEntity(FeaturedProduct? entity = default)
        {
            entity ??= new FeaturedProduct();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.ProductId = ProductId;
            entity.Position = Position;
            entity.IsSponsored = IsSponsored;

            return entity;
        }

        public FeaturedProductDocument PopulateFromEntity(FeaturedProduct entity)
        {
            Id = entity.Id;
            ProductId = entity.ProductId;
            Position = entity.Position;
            IsSponsored = entity.IsSponsored;

            return this;
        }

        public static FeaturedProductDocument? FromEntity(FeaturedProduct? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new FeaturedProductDocument().PopulateFromEntity(entity);
        }
    }
}