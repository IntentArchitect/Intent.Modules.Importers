using System;
using System.ComponentModel.DataAnnotations.Schema;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DataContract", Version = "1.0")]

namespace RdbmsImporterTests.Domain.Contracts.Public
{
    public record UuidNsX500Response
    {
        public UuidNsX500Response(Guid? uuidNsX500)
        {
            UuidNsX500 = uuidNsX500;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        [IntentManaged(Mode.Fully)]
        protected UuidNsX500Response()
        {
        }

        [Column("uuid_ns_x500")]
        public Guid? UuidNsX500 { get; init; }
    }
}