using System;
using System.ComponentModel.DataAnnotations.Schema;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DataContract", Version = "1.0")]

namespace RdbmsImporterTests.Domain.Contracts.Public
{
    public record UuidNsDnsResponse
    {
        public UuidNsDnsResponse(Guid? uuidNsDns)
        {
            UuidNsDns = uuidNsDns;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        [IntentManaged(Mode.Fully)]
        protected UuidNsDnsResponse()
        {
        }

        [Column("uuid_ns_dns")]
        public Guid? UuidNsDns { get; init; }
    }
}