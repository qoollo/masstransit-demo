using GreenPipes;
using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;

namespace OrderOrchestratorService.Consumers
{
    public class GetOrderStateConsumerDefinition : ConsumerDefinition<GetOrderStateConsumer>
    {
        public GetOrderStateConsumerDefinition()
        {

        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<GetOrderStateConsumer> consumerConfigurator)
        {
            consumerConfigurator.UseDelayedRedelivery(r => r.Intervals(1000, 2000, 5000, 10000, 10000));
            consumerConfigurator.UseMessageRetry((r => r.Intervals(1000, 2000, 5000, 10000, 10000)));
            consumerConfigurator.UseInMemoryOutbox();
        }
    }
}
