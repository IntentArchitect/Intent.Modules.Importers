using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.ECommerce.Catalog.Products
{
    public class ProductImage
    {
        private Guid? _id;

        public ProductImage()
        {
            Url = null!;
            AltText = null!;
            ImageType = null!;
        }

        public Guid Id
        {
            get => _id ??= Guid.NewGuid();
            set => _id = value;
        }

        public string Url { get; set; }

        public string AltText { get; set; }

        public decimal Position { get; set; }

        public bool IsPrimary { get; set; }

        public string ImageType { get; set; }
    }
}