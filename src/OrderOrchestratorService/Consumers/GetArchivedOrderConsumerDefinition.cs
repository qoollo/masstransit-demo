using GreenPipes;
using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;

namespace OrderOrchestratorService.Consumers
{
    public class GetArchivedOrderConsumerDefinition : ConsumerDefinition<GetArchivedOrderConsumer>
    {
        public GetArchivedOrderConsumerDefinition()
        {

        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<GetArchivedOrderConsumer> consumerConfigurator)
        {
            consumerConfigurator.UseDelayedRedelivery(r => r.Intervals(1000, 2000, 5000, 10000, 10000));
            consumerConfigurator.UseMessageRetry((r => r.Intervals(1000, 2000, 5000, 10000, 10000)));
            consumerConfigurator.UseInMemoryOutbox();
        }
    }
}
