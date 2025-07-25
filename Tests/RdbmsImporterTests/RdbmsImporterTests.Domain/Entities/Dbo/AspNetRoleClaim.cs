using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using RdbmsImporterTests.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace RdbmsImporterTests.Domain.Entities.Dbo
{
    public class AspNetRoleClaim : IHasDomainEvent
    {
        public AspNetRoleClaim()
        {
            RoleId = null!;
            RoleIdAspNetRoles = null!;
        }
        public int Id { get; set; }

        public string RoleId { get; set; }

        public string? ClaimType { get; set; }

        public string? ClaimValue { get; set; }

        public virtual AspNetRole RoleIdAspNetRoles { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}