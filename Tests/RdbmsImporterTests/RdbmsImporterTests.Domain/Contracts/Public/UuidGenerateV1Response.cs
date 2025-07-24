using System;
using System.ComponentModel.DataAnnotations.Schema;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DataContract", Version = "1.0")]

namespace RdbmsImporterTests.Domain.Contracts.Public
{
    public record UuidGenerateV1Response
    {
        public UuidGenerateV1Response(Guid? uuidGenerateV1)
        {
            UuidGenerateV1 = uuidGenerateV1;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        [IntentManaged(Mode.Fully)]
        protected UuidGenerateV1Response()
        {
        }

        [Column("uuid_generate_v1")]
        public Guid? UuidGenerateV1 { get; init; }
    }
}