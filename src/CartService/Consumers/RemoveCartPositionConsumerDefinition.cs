using GreenPipes;
using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Consumers
{
    public class RemoveCartPositionConsumerDefinition : ConsumerDefinition<RemoveCartPositionConsumer>
    {
        public RemoveCartPositionConsumerDefinition()
        {

        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<RemoveCartPositionConsumer> consumerConfigurator)
        {
            consumerConfigurator.UseDelayedRedelivery(r => r.Incremental(5, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2)));
            consumerConfigurator.UseMessageRetry(r => r.Immediate(5));
            consumerConfigurator.UseInMemoryOutbox();
        }
    }
}
