using System.ComponentModel.DataAnnotations.Schema;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DataContract", Version = "1.0")]

namespace RdbmsImporterTests.Domain.Contracts.Public
{
    public record GetCustomerOrdersResponse
    {
        public GetCustomerOrdersResponse(string? result)
        {
            Result = result;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        [IntentManaged(Mode.Fully)]
        protected GetCustomerOrdersResponse()
        {
        }

        [Column("result")]
        public string? Result { get; init; }
    }
}