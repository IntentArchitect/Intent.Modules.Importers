using System;
using System.ComponentModel.DataAnnotations.Schema;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DataContract", Version = "1.0")]

namespace RdbmsImporterTests.Domain.Contracts.Public
{
    public record UuidGenerateV4Response
    {
        public UuidGenerateV4Response(Guid? uuidGenerateV4)
        {
            UuidGenerateV4 = uuidGenerateV4;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        [IntentManaged(Mode.Fully)]
        protected UuidGenerateV4Response()
        {
        }

        [Column("uuid_generate_v4")]
        public Guid? UuidGenerateV4 { get; init; }
    }
}