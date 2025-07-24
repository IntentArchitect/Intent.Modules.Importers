using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using RdbmsImporterTests.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace RdbmsImporterTests.Domain.Entities.Public
{
    public class AspNetUserLogin : IHasDomainEvent
    {
        public AspNetUserLogin()
        {
            LoginProvider = null!;
            ProviderKey = null!;
            UserId = null!;
            UserAspNetUser = null!;
        }

        public string LoginProvider { get; set; }

        public string ProviderKey { get; set; }

        public string? ProviderDisplayName { get; set; }

        public string UserId { get; set; }

        public virtual AspNetUser UserAspNetUser { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}