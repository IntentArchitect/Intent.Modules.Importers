using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using RdbmsImporterTests.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace RdbmsImporterTests.Domain.Entities.Public
{
    public class Address : IHasDomainEvent
    {
        public Address()
        {
            Line1 = null!;
            Line2 = null!;
            City = null!;
            PostalCode = null!;
            Customer = null!;
        }

        public Guid Id { get; set; }

        public Guid CustomerId { get; set; }

        public string Line1 { get; set; }

        public string Line2 { get; set; }

        public string City { get; set; }

        public string PostalCode { get; set; }

        public int AddressType { get; set; }

        public virtual Customer Customer { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}