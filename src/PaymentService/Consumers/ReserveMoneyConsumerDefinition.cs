using GreenPipes;
using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;

namespace PaymentService.Consumers
{
    public class ReserveMoneyConsumerDefinition : ConsumerDefinition<ReserveMoneyConsumer>
    {
        public ReserveMoneyConsumerDefinition()
        {
            
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<ReserveMoneyConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseDelayedRedelivery(r => r.Interval(5, 1000));
            endpointConfigurator.UseMessageRetry(r => r.Interval(5, 5000));
            endpointConfigurator.UseInMemoryOutbox();
        }
    }
}
