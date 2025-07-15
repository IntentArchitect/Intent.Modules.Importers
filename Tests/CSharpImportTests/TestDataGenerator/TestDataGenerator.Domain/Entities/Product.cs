using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using TestDataGenerator.Domain.Common;
using TestDataGenerator.Domain.Enums;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace TestDataGenerator.Domain.Entities
{
    public class Product : IHasDomainEvent
    {
        public Product()
        {
            Name = null!;
        }
        public Guid Id { get; set; }

        public string Name { get; set; }

        public ProductType Type { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}