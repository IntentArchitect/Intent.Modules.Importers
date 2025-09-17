using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Application.Common.Eventing;
using JsonImportTests.Domain.Common.Interfaces;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.IntegrationEventConsumer", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Eventing
{
    public class IntegrationEventConsumer<THandler, TMessage> : IConsumer<TMessage>
        where TMessage : class
        where THandler : IIntegrationEventHandler<TMessage>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ICosmosDBUnitOfWork _cosmosDBUnitOfWork;

        public IntegrationEventConsumer(IServiceProvider serviceProvider, ICosmosDBUnitOfWork cosmosDBUnitOfWork)
        {
            _serviceProvider = serviceProvider;
            _cosmosDBUnitOfWork = cosmosDBUnitOfWork ?? throw new ArgumentNullException(nameof(cosmosDBUnitOfWork));
        }

        public async Task Consume(ConsumeContext<TMessage> context)
        {
            var eventBus = _serviceProvider.GetRequiredService<MassTransitEventBus>();
            eventBus.ConsumeContext = context;
            var handler = _serviceProvider.GetRequiredService<THandler>();
            await handler.HandleAsync(context.Message, context.CancellationToken);
            await _cosmosDBUnitOfWork.SaveChangesAsync(context.CancellationToken);
            await eventBus.FlushAllAsync(context.CancellationToken);
        }
    }

    public class IntegrationEventConsumerDefinition<THandler, TMessage> : ConsumerDefinition<IntegrationEventConsumer<THandler, TMessage>>
        where TMessage : class
        where THandler : IIntegrationEventHandler<TMessage>
    {
        protected override void ConfigureConsumer(
            IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<IntegrationEventConsumer<THandler, TMessage>> consumerConfigurator,
            IRegistrationContext context)
        {
            endpointConfigurator.UseInMemoryInboxOutbox(context);
        }
    }
}