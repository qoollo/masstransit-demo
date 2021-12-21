using GreenPipes;
using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;

namespace ApiService.Consumers
{
    public class FeedbackRequestedConsumerDefinition : ConsumerDefinition<FeedbackRequestedConsumer>
    {
        public FeedbackRequestedConsumerDefinition()
        {

        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<FeedbackRequestedConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseDelayedRedelivery(r => r.Interval(5, 1000));
            endpointConfigurator.UseMessageRetry(r => r.Interval(5, 5000));
            endpointConfigurator.UseInMemoryOutbox();
        }
    }
}
