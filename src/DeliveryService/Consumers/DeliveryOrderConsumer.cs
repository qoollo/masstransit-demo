using System;
using System.Threading.Tasks;
using DeliveryService.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace DeliveryService.Consumers
{
    public class DeliveryOrderConsumer : IConsumer<DeliveryOrder>
    {
        private readonly ILogger<DeliveryOrderConsumer> _logger;

        public DeliveryOrderConsumer(ILogger<DeliveryOrderConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<DeliveryOrder> context)
        {
            _logger.LogInformation("[{consumerName}] Received delivery request for order {orderId}.",
                nameof(DeliveryOrderConsumer), context.Message.OrderId);


            await context.SchedulePublish<OrderDelivered>(DateTime.UtcNow + TimeSpan.FromSeconds(30),
                new
                {
                    OrderId = context.Message.OrderId
                });
        }
    }
}