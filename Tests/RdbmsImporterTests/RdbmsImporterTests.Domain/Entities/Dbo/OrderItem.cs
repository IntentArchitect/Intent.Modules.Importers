using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using RdbmsImporterTests.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace RdbmsImporterTests.Domain.Entities.Dbo
{
    public class OrderItem : IHasDomainEvent
    {
        public OrderItem()
        {
            Order = null!;
            Product = null!;
        }
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        public int Quantity { get; set; }

        public decimal Amount { get; set; }

        public Guid ProductId { get; set; }

        public virtual Order Order { get; set; }

        public virtual Product Product { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}