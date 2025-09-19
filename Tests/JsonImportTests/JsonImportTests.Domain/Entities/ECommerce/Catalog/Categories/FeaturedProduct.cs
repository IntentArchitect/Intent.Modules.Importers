using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.ECommerce.Catalog.Categories
{
    public class FeaturedProduct
    {
        private string? _id;

        public FeaturedProduct()
        {
            Id = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public Guid ProductId { get; set; }

        public decimal Position { get; set; }

        public bool IsSponsored { get; set; }
    }
}