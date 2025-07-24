using System;
using System.ComponentModel.DataAnnotations.Schema;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DataContract", Version = "1.0")]

namespace RdbmsImporterTests.Domain.Contracts.Public
{
    public record UuidGenerateV1mcResponse
    {
        public UuidGenerateV1mcResponse(Guid? uuidGenerateV1mc)
        {
            UuidGenerateV1mc = uuidGenerateV1mc;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        [IntentManaged(Mode.Fully)]
        protected UuidGenerateV1mcResponse()
        {
        }

        [Column("uuid_generate_v1mc")]
        public Guid? UuidGenerateV1mc { get; init; }
    }
}