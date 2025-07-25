using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using RdbmsImporterTests.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace RdbmsImporterTests.Domain.Entities.Public
{
    public class Child : IHasDomainEvent
    {
        public Child()
        {
            Parent = null!;
        }

        public Guid Id { get; set; }

        public Guid ParentId { get; set; }

        public Guid ParentId2 { get; set; }

        public virtual Parent Parent { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}