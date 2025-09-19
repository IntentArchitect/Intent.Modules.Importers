using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.ECommerce.Catalog.Products
{
    public class Variant
    {
        private Guid? _id;

        public Variant()
        {
            SKU = null!;
            Name = null!;
            Pricing = null!;
            Inventory = null!;
            Dimensions = null!;
            Attributes = null!;
        }

        public Guid Id
        {
            get => _id ??= Guid.NewGuid();
            set => _id = value;
        }

        public string SKU { get; set; }

        public string Name { get; set; }

        public VariantsPricing Pricing { get; set; }

        public VariantsInventory Inventory { get; set; }

        public ICollection<VariantsImage> Images { get; set; } = [];

        public VariantsDimension Dimensions { get; set; }

        public VariantsAttribute Attributes { get; set; }
    }
}