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
    public class GetCartConsumerDefinition : ConsumerDefinition<GetCartConsumer>
    {
        public GetCartConsumerDefinition()
        {

        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<GetCartConsumer> consumerConfigurator)
        {
            consumerConfigurator.UseDelayedRedelivery(r => r.Exponential(5, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(12), TimeSpan.FromSeconds(1.5)));
            consumerConfigurator.UseMessageRetry(r => r.Intervals(1000, 2000, 5000, 10000, 10000));
            consumerConfigurator.UseInMemoryOutbox();
        }
    }
}
