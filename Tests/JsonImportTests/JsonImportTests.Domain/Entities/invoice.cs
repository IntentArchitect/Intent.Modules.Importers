using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities
{
    public class invoice
    {
        private Guid? _id;

        public Guid Id
        {
            get => _id ??= Guid.NewGuid();
            set => _id = value;
        }

        public Guid CustomerId { get; set; }

        public decimal TotalAmount { get; set; }

        public ICollection<Item> Items { get; set; } = [];
    }
}