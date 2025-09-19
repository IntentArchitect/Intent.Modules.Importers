using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.ECommerce.Catalog.Products
{
    public class Product
    {
        private Guid? _id;

        public Product()
        {
            SKU = null!;
            Name = null!;
            Description = null!;
            ShortDescription = null!;
            ProductType = null!;
            Status = null!;
            CreatedBy = null!;
            Shipping = null!;
            SEO = null!;
            Reviews = null!;
            Pricing = null!;
            Category = null!;
            Brand = null!;
        }

        public Guid Id
        {
            get => _id ??= Guid.NewGuid();
            set => _id = value;
        }

        public string SKU { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ShortDescription { get; set; }

        public string ProductType { get; set; }

        public string Status { get; set; }

        public IList<string> Tags { get; set; } = [];

        public DateTime CreatedDate { get; set; }

        public DateTime LastModified { get; set; }

        public string CreatedBy { get; set; }

        public bool IsActive { get; set; }

        public bool IsVisible { get; set; }

        public bool IsFeatured { get; set; }

        public ICollection<Variant> Variants { get; set; } = [];

        public Shipping Shipping { get; set; }

        public ProductSEO SEO { get; set; }

        public Review Reviews { get; set; }

        public ICollection<RelatedProduct> RelatedProducts { get; set; } = [];

        public ProductPricing Pricing { get; set; }

        public ICollection<ProductImage> Images { get; set; } = [];

        public ProductCategory Category { get; set; }

        public Brand Brand { get; set; }

        public ICollection<ProductAttribute> Attributes { get; set; } = [];
    }
}