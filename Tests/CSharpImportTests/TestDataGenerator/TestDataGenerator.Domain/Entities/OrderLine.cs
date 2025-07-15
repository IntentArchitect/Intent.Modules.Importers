using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace TestDataGenerator.Domain.Entities
{
    public class OrderLine
    {
        public OrderLine()
        {
            Description = null!;
            Product = null!;
        }
        public Guid Id { get; set; }

        public decimal Amount { get; set; }

        public string Description { get; set; }

        public Guid OrderId { get; set; }

        public Guid ProductId { get; set; }

        public virtual Product Product { get; set; }
    }
}