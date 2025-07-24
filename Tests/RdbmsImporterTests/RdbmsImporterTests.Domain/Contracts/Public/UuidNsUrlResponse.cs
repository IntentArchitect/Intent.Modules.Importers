using System;
using System.ComponentModel.DataAnnotations.Schema;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DataContract", Version = "1.0")]

namespace RdbmsImporterTests.Domain.Contracts.Public
{
    public record UuidNsUrlResponse
    {
        public UuidNsUrlResponse(Guid? uuidNsUrl)
        {
            UuidNsUrl = uuidNsUrl;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        [IntentManaged(Mode.Fully)]
        protected UuidNsUrlResponse()
        {
        }

        [Column("uuid_ns_url")]
        public Guid? UuidNsUrl { get; init; }
    }
}