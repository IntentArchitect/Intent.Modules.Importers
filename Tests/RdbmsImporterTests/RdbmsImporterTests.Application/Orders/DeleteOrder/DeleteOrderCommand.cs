using System;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using RdbmsImporterTests.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace RdbmsImporterTests.Application.Orders.DeleteOrder
{
    public class DeleteOrderCommand : IRequest, ICommand
    {
        public DeleteOrderCommand(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}