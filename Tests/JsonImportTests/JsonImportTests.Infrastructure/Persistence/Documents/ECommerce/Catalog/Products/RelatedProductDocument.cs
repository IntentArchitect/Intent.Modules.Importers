using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.ECommerce.Catalog.Products;
using JsonImportTests.Domain.Repositories.Documents.ECommerce.Catalog.Products;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.ECommerce.Catalog.Products
{
    internal class RelatedProductDocument : IRelatedProductDocument
    {
        public string Id { get; set; } = default!;
        public Guid ProductId { get; set; }
        public string RelationType { get; set; } = default!;
        public decimal Position { get; set; }

        public RelatedProduct ToEntity(RelatedProduct? entity = default)
        {
            entity ??= new RelatedProduct();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.ProductId = ProductId;
            entity.RelationType = RelationType ?? throw new Exception($"{nameof(entity.RelationType)} is null");
            entity.Position = Position;

            return entity;
        }

        public RelatedProductDocument PopulateFromEntity(RelatedProduct entity)
        {
            Id = entity.Id;
            ProductId = entity.ProductId;
            RelationType = entity.RelationType;
            Position = entity.Position;

            return this;
        }

        public static RelatedProductDocument? FromEntity(RelatedProduct? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new RelatedProductDocument().PopulateFromEntity(entity);
        }
    }
}