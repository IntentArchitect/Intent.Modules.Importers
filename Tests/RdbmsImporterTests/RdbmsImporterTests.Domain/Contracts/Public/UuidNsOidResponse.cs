using System;
using System.ComponentModel.DataAnnotations.Schema;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DataContract", Version = "1.0")]

namespace RdbmsImporterTests.Domain.Contracts.Public
{
    public record UuidNsOidResponse
    {
        public UuidNsOidResponse(Guid? uuidNsOid)
        {
            UuidNsOid = uuidNsOid;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        [IntentManaged(Mode.Fully)]
        protected UuidNsOidResponse()
        {
        }

        [Column("uuid_ns_oid")]
        public Guid? UuidNsOid { get; init; }
    }
}