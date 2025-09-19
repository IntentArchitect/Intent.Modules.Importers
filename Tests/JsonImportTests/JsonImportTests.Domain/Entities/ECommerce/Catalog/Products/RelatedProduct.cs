using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.ECommerce.Catalog.Products
{
    public class RelatedProduct
    {
        private string? _id;

        public RelatedProduct()
        {
            Id = null!;
            RelationType = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public Guid ProductId { get; set; }

        public string RelationType { get; set; }

        public decimal Position { get; set; }
    }
}