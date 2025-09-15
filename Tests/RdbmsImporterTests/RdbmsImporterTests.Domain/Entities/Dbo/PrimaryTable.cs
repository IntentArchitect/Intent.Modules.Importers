using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using RdbmsImporterTests.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace RdbmsImporterTests.Domain.Entities.Dbo
{
    public class PrimaryTable : IHasDomainEvent
    {
        public PrimaryTable()
        {
            Name = null!;
            FKWithTableId2FKTable = null!;
            FKTryTableId4FKTable = null!;
            FKThisTableId5FKTable = null!;
            FKTableId1FKTable = null!;
            FKAsTableId3FKTable = null!;
        }

        public int PrimaryTableId { get; set; }

        public string Name { get; set; }

        public int FKTableId1 { get; set; }

        public int FKWithTableId2 { get; set; }

        public int FKTryTableId4 { get; set; }

        public int FKThisTableId5 { get; set; }

        public int FKAsTableId3 { get; set; }

        public virtual FKTable FKWithTableId2FKTable { get; set; }

        public virtual FKTable FKTryTableId4FKTable { get; set; }

        public virtual FKTable FKThisTableId5FKTable { get; set; }

        public virtual FKTable FKTableId1FKTable { get; set; }

        public virtual FKTable FKAsTableId3FKTable { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}