using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using TestDataGenerator.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace TestDataGenerator.Domain.Entities.RepoTest
{
    public class AggregateRoot1 : IHasDomainEvent
    {
        public AggregateRoot1()
        {
            Tag = null!;
        }
        public Guid Id { get; set; }

        /// <summary>
        /// Here is a multi
        /// line comment that
        /// is supposed to work for "HasComment()"
        /// and has some quotes included
        /// </summary>
        public string Tag { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];

        public AggregateRoot1 Operation(object param1)
        {
            // [IntentFully]
            // TODO: Implement Operation (AggregateRoot1) functionality
            throw new NotImplementedException("Replace with your implementation...");
        }
    }
}