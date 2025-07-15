using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using TestDataGenerator.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainServices.DomainServiceInterface", Version = "1.0")]

namespace TestDataGenerator.Domain.Services
{
    public interface IOrderDomainService
    {
        Order GetOrder(Guid id);
        Task<Order> GetOrderAsync(Guid id, CancellationToken cancellationToken = default);
    }
}