using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.ECommerce.Catalog.Products
{
    public class Brand
    {
        public Brand()
        {
            Name = null!;
            LogoUrl = null!;
            Website = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string LogoUrl { get; set; }

        public string Website { get; set; }
    }
}