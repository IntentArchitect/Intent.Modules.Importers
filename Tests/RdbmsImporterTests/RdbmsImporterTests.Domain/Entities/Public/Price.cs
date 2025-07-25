using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using RdbmsImporterTests.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace RdbmsImporterTests.Domain.Entities.Public
{
    public class Price : IHasDomainEvent
    {
        public Price()
        {
            Product = null!;
        }

        public Guid Id { get; set; }

        public Guid ProductId { get; set; }

        public DateTime ActiveFrom { get; set; }

        public decimal Amount { get; set; }

        public virtual Product Product { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}