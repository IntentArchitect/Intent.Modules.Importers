using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using RdbmsImporterTests.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace RdbmsImporterTests.Domain.Entities.Schema2
{
    public class Customer : IHasDomainEvent
    {
        public Customer()
        {
            Name = null!;
            Surname = null!;
        }
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}