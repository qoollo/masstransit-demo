using GreenPipes;
using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeedbackService.Consumers
{
    public class AddFeedbackConsumerDefinition : ConsumerDefinition<AddFeedbackConsumer>
    {
        public AddFeedbackConsumerDefinition()
        {

        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<AddFeedbackConsumer> consumerConfigurator)
        {
            consumerConfigurator.UseDelayedRedelivery(r => r.Intervals(1000, 2000, 5000, 10000, 10000));
            consumerConfigurator.UseMessageRetry((r => r.Intervals(1000, 2000, 5000, 10000, 10000)));
            consumerConfigurator.UseInMemoryOutbox();
        }
    }
}
