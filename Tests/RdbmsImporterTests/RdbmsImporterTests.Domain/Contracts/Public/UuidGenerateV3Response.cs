using System;
using System.ComponentModel.DataAnnotations.Schema;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DataContract", Version = "1.0")]

namespace RdbmsImporterTests.Domain.Contracts.Public
{
    public record UuidGenerateV3Response
    {
        public UuidGenerateV3Response(Guid? uuidGenerateV3)
        {
            UuidGenerateV3 = uuidGenerateV3;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        [IntentManaged(Mode.Fully)]
        protected UuidGenerateV3Response()
        {
        }

        [Column("uuid_generate_v3")]
        public Guid? UuidGenerateV3 { get; init; }
    }
}