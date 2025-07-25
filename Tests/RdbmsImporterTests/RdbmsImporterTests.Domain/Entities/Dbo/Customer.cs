using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using RdbmsImporterTests.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace RdbmsImporterTests.Domain.Entities.Dbo
{
    public class Customer : IHasDomainEvent
    {
        public Customer()
        {
            Name = null!;
            Surname = null!;
            Email = null!;
        }
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string Email { get; set; }

        public bool IsActive { get; set; }

        public bool? PreferencesSpecials { get; set; }

        public bool? PreferencesNewsletter { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}