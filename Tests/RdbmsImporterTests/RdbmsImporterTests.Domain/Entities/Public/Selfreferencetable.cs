using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using RdbmsImporterTests.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace RdbmsImporterTests.Domain.Entities.Public
{
    public class Selfreferencetable : IHasDomainEvent
    {
        public Selfreferencetable()
        {
            Name = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string? Email { get; set; }

        public Guid? Managerid { get; set; }

        public virtual Selfreferencetable? Managerselfreferencetable { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}