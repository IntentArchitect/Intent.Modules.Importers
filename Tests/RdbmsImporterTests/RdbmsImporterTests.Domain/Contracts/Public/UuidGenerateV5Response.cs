using System;
using System.ComponentModel.DataAnnotations.Schema;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DataContract", Version = "1.0")]

namespace RdbmsImporterTests.Domain.Contracts.Public
{
    public record UuidGenerateV5Response
    {
        public UuidGenerateV5Response(Guid? uuidGenerateV5)
        {
            UuidGenerateV5 = uuidGenerateV5;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        [IntentManaged(Mode.Fully)]
        protected UuidGenerateV5Response()
        {
        }

        [Column("uuid_generate_v5")]
        public Guid? UuidGenerateV5 { get; init; }
    }
}