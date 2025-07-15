using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using TestDataGenerator.Application.Orders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace TestDataGenerator.Application.Interfaces
{
    public interface IOrdersService
    {
        Task<Guid> CreateOrder(OrderCreateDto dto, CancellationToken cancellationToken = default);
        Task<OrderDto> FindOrderById(Guid id, CancellationToken cancellationToken = default);
        Task<List<OrderDto>> FindOrders(CancellationToken cancellationToken = default);
        Task UpdateOrder(Guid id, OrderUpdateDto dto, CancellationToken cancellationToken = default);
        Task DeleteOrder(Guid id, CancellationToken cancellationToken = default);
    }
}