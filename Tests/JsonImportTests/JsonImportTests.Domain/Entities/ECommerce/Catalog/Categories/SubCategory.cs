using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.ECommerce.Catalog.Categories
{
    public class SubCategory
    {
        private Guid? _id;

        public SubCategory()
        {
            Name = null!;
            Slug = null!;
        }

        public Guid Id
        {
            get => _id ??= Guid.NewGuid();
            set => _id = value;
        }

        public string Name { get; set; }

        public string Slug { get; set; }

        public decimal ProductCount { get; set; }

        public bool IsActive { get; set; }
    }
}