using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using RdbmsImporterTests.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace RdbmsImporterTests.Domain.Entities.Public
{
    public class AspNetRole : IHasDomainEvent
    {
        public AspNetRole()
        {
            Id = null!;
        }

        public string Id { get; set; }

        public string? Name { get; set; }

        public string? NormalizedName { get; set; }

        public string? ConcurrencyStamp { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}