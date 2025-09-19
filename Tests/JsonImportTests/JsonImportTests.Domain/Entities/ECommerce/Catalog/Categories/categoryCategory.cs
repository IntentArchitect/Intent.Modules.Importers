using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.ECommerce.Catalog.Categories
{
    public class categoryCategory
    {
        private Guid? _id;

        public categoryCategory()
        {
            Name = null!;
            Description = null!;
            Slug = null!;
            Path = null!;
            CreatedBy = null!;
            Metadata = null!;
        }

        public Guid Id
        {
            get => _id ??= Guid.NewGuid();
            set => _id = value;
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Slug { get; set; }

        public Guid ParentCategoryId { get; set; }

        public decimal Level { get; set; }

        public string Path { get; set; }

        public decimal DisplayOrder { get; set; }

        public bool IsActive { get; set; }

        public bool IsVisible { get; set; }

        public decimal ProductCount { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime LastModified { get; set; }

        public string CreatedBy { get; set; }

        public ICollection<SubCategory> SubCategories { get; set; } = [];

        public CategoryMetadata Metadata { get; set; }

        public ICollection<Filter> Filters { get; set; } = [];

        public ICollection<FeaturedProduct> FeaturedProducts { get; set; } = [];

        public ICollection<CategoryAttribute> Attributes { get; set; } = [];
    }
}