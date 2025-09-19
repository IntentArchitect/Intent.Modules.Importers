using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.ECommerce.Catalog.Products
{
    public class ProductCategory
    {
        public ProductCategory()
        {
            Name = null!;
        }

        public Guid CategoryId { get; set; }

        public string Name { get; set; }

        public Guid ParentCategoryId { get; set; }
    }
}