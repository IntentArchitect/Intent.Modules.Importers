using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using RdbmsImporterTests.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace RdbmsImporterTests.Domain.Entities.Public
{
    public class AspNetUserClaim : IHasDomainEvent
    {
        public AspNetUserClaim()
        {
            UserId = null!;
            UserAspNetUser = null!;
        }

        public int Id { get; set; }

        public string UserId { get; set; }

        public string? ClaimType { get; set; }

        public string? ClaimValue { get; set; }

        public virtual AspNetUser UserAspNetUser { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}