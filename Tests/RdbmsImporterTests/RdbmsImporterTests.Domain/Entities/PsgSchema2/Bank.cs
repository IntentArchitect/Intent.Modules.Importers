using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using RdbmsImporterTests.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace RdbmsImporterTests.Domain.Entities.PsgSchema2
{
    public class Bank : IHasDomainEvent
    {
        public Bank()
        {
            Name = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}