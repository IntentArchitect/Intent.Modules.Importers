using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities
{
    public class customer
    {
        private Guid? _id;

        public customer()
        {
            Name = null!;
            Email = null!;
        }

        public Guid Id
        {
            get => _id ??= Guid.NewGuid();
            set => _id = value;
        }

        public string Name { get; set; }

        public string Email { get; set; }

        public ICollection<Order> Orders { get; set; } = [];
    }
}