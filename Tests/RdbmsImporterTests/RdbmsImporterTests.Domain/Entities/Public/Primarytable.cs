using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using RdbmsImporterTests.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace RdbmsImporterTests.Domain.Entities.Public
{
    public class Primarytable : IHasDomainEvent
    {
        public Primarytable()
        {
            Name = null!;
            Fkwithtableid2fktable = null!;
            Fktrytableid4fktable = null!;
            Fkthistableid5fktable = null!;
            Fktableid1fktable = null!;
            Fkastableid3fktable = null!;
        }

        public int Primarytableid { get; set; }

        public string Name { get; set; }

        public int Fktableid1 { get; set; }

        public int Fkwithtableid2 { get; set; }

        public int Fktrytableid4 { get; set; }

        public int Fkthistableid5 { get; set; }

        public int Fkastableid3 { get; set; }

        public virtual Fktable Fkwithtableid2fktable { get; set; }

        public virtual Fktable Fktrytableid4fktable { get; set; }

        public virtual Fktable Fkthistableid5fktable { get; set; }

        public virtual Fktable Fktableid1fktable { get; set; }

        public virtual Fktable Fkastableid3fktable { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}