using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.ECommerce.Catalog.Products;
using JsonImportTests.Domain.Repositories.Documents.ECommerce.Catalog.Products;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.ECommerce.Catalog.Products
{
    internal class ProductCategoryDocument : IProductCategoryDocument
    {
        public Guid CategoryId { get; set; }
        public string Name { get; set; } = default!;
        public Guid ParentCategoryId { get; set; }

        public ProductCategory ToEntity(ProductCategory? entity = default)
        {
            entity ??= new ProductCategory();

            entity.CategoryId = CategoryId;
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");
            entity.ParentCategoryId = ParentCategoryId;

            return entity;
        }

        public ProductCategoryDocument PopulateFromEntity(ProductCategory entity)
        {
            CategoryId = entity.CategoryId;
            Name = entity.Name;
            ParentCategoryId = entity.ParentCategoryId;

            return this;
        }

        public static ProductCategoryDocument? FromEntity(ProductCategory? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new ProductCategoryDocument().PopulateFromEntity(entity);
        }
    }
}