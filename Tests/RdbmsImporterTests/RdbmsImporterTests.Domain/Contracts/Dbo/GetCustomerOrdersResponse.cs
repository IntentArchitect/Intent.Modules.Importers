using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DataContract", Version = "1.0")]

namespace RdbmsImporterTests.Domain.Contracts.Dbo
{
    public record GetCustomerOrdersResponse
    {
        public GetCustomerOrdersResponse(DateTime orderDate, string refNo)
        {
            OrderDate = orderDate;
            RefNo = refNo;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        [IntentManaged(Mode.Fully)]
        protected GetCustomerOrdersResponse()
        {
            RefNo = null!;
        }

        public DateTime OrderDate { get; init; }
        public string RefNo { get; init; }
    }
}