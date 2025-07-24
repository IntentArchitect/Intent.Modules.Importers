using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using RdbmsImporterTests.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace RdbmsImporterTests.Domain.Entities.Public
{
    public class AspNetUserRole : IHasDomainEvent
    {
        public AspNetUserRole()
        {
            UserId = null!;
            RoleId = null!;
            UserAspNetUser = null!;
            RoleAspNetRole = null!;
        }

        public string UserId { get; set; }

        public string RoleId { get; set; }

        public virtual AspNetUser UserAspNetUser { get; set; }

        public virtual AspNetRole RoleAspNetRole { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}