using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.ECommerce.Catalog.Categories
{
    public class CategoryAttribute
    {
        private string? _id;

        public CategoryAttribute()
        {
            Id = null!;
            Name = null!;
            Type = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public Guid AttributeId { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public bool IsRequired { get; set; }

        public bool IsFilterable { get; set; }

        public ICollection<AttributeValue> AttributeValues { get; set; } = [];
    }
}