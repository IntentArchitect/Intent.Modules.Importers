using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.ECommerce.Catalog.Categories
{
    public class AttributeValue
    {
        private string? _id;

        public AttributeValue()
        {
            Id = null!;
            Value = null!;
            DisplayName = null!;
            Color = null!;
            Image = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public string Value { get; set; }

        public string DisplayName { get; set; }

        public string Color { get; set; }

        public string Image { get; set; }
    }
}