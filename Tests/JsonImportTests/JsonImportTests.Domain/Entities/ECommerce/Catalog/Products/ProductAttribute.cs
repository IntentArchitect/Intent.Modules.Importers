using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.ECommerce.Catalog.Products
{
    public class ProductAttribute
    {
        private string? _id;

        public ProductAttribute()
        {
            Id = null!;
            Name = null!;
            Value = null!;
            Type = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public string Name { get; set; }

        public string Value { get; set; }

        public string Type { get; set; }

        public bool IsFilterable { get; set; }

        public bool IsSearchable { get; set; }

        public decimal DisplayOrder { get; set; }
    }
}