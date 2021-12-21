using GreenPipes;
using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;

namespace ApiService.Consumers
{
    public class GetArchivedOrderResponseConsumerDefinition : ConsumerDefinition<GetArchivedOrderResponseConsumer>
    {
        public GetArchivedOrderResponseConsumerDefinition()
        {

        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<GetArchivedOrderResponseConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseDelayedRedelivery(r => r.Interval(5, 1000));
            endpointConfigurator.UseMessageRetry(r => r.Interval(5, 5000));
            endpointConfigurator.UseInMemoryOutbox();
        }
    }
}
