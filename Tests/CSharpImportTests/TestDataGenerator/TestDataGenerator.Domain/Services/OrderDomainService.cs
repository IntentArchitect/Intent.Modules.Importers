using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using TestDataGenerator.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainServices.DomainServiceImplementation", Version = "1.0")]

namespace TestDataGenerator.Domain.Services
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class OrderDomainService : IOrderDomainService
    {
        [IntentManaged(Mode.Merge, Body = Mode.Ignore)]
        public OrderDomainService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public Order GetOrder(Guid id)
        {
            // TODO: Implement GetOrder (OrderDomainService) functionality
            throw new NotImplementedException("Implement your domain service logic here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<Order> GetOrderAsync(Guid id, CancellationToken cancellationToken = default)
        {
            // TODO: Implement GetOrderAsync (OrderDomainService) functionality
            throw new NotImplementedException("Implement your domain service logic here...");
        }
    }
}