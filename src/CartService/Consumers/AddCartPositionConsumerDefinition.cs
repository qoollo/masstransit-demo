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
    public class AddCartPositionConsumerDefinition : ConsumerDefinition<AddCartPositionConsumer>
    {
        public AddCartPositionConsumerDefinition()
        {

        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<AddCartPositionConsumer> consumerConfigurator)
        {
            consumerConfigurator.UseDelayedRedelivery(r => r.Immediate(5));
            consumerConfigurator.UseMessageRetry(r => r.Incremental(5, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2)));
            consumerConfigurator.UseInMemoryOutbox();
        }
    }
}
