using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using RdbmsImporterTests.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace RdbmsImporterTests.Domain.Entities.Dbo
{
    public class LegacyTable : IHasDomainEvent
    {
        public LegacyTable()
        {
            LegacyColumn = null!;
        }
        public int LegacyId { get; set; }

        public string LegacyColumn { get; set; }

        public DateTime BadDate { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}