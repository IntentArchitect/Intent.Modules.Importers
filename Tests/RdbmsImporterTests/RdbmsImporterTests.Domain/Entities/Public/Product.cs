using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using RdbmsImporterTests.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace RdbmsImporterTests.Domain.Entities.Public
{
    public class Product : IHasDomainEvent
    {
        public Product()
        {
            Name = null!;
            Description = null!;
            Brand = null!;
        }

        public Guid Id { get; set; }

        public Guid BrandId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public virtual Brand Brand { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}