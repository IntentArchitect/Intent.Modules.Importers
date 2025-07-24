using System;
using System.ComponentModel.DataAnnotations.Schema;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DataContract", Version = "1.0")]

namespace RdbmsImporterTests.Domain.Contracts.Public
{
    public record UuidNilResponse
    {
        public UuidNilResponse(Guid? uuidNil)
        {
            UuidNil = uuidNil;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        [IntentManaged(Mode.Fully)]
        protected UuidNilResponse()
        {
        }

        [Column("uuid_nil")]
        public Guid? UuidNil { get; init; }
    }
}