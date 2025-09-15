using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using RdbmsImporterTests.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace RdbmsImporterTests.Domain.Entities.Dbo
{
    public class SelfReferenceTable : IHasDomainEvent
    {
        public SelfReferenceTable()
        {
            Name = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string? Email { get; set; }

        public Guid? ManagerId { get; set; }

        public virtual SelfReferenceTable? ManagerSelfReferenceTable { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}