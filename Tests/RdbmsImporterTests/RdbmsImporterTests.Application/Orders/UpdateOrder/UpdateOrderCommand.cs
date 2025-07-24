using System;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using RdbmsImporterTests.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace RdbmsImporterTests.Application.Orders.UpdateOrder
{
    public class UpdateOrderCommand : IRequest, ICommand
    {
        public UpdateOrderCommand(Guid id, Guid customerId, DateTime orderDate, string refNo)
        {
            Id = id;
            CustomerId = customerId;
            OrderDate = orderDate;
            RefNo = refNo;
        }

        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public string RefNo { get; set; }
    }
}