using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities
{
    public class Item
    {
        private string? _id;

        public Item()
        {
            Id = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public Guid ProductId { get; set; }

        public decimal Quantity { get; set; }

        public decimal Price { get; set; }
    }
}