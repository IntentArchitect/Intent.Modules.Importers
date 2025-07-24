using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using RdbmsImporterTests.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace RdbmsImporterTests.Domain.Entities.Public
{
    public class AspNetUserToken : IHasDomainEvent
    {
        public AspNetUserToken()
        {
            UserId = null!;
            LoginProvider = null!;
            Name = null!;
            UserAspNetUser = null!;
        }

        public string UserId { get; set; }

        public string LoginProvider { get; set; }

        public string Name { get; set; }

        public string? Value { get; set; }

        public virtual AspNetUser UserAspNetUser { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}