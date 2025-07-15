using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using TestDataGenerator.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace TestDataGenerator.Domain.Entities
{
    public class Order : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public int RefNo { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public virtual ICollection<OrderLine> OrderLines { get; set; } = [];

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}