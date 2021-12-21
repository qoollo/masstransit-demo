using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using PaymentService.Contracts;

namespace PaymentService.Consumers
{
    public class UnreserveMoneyConsumer : IConsumer<UnreserveMoney>
    {
        private readonly ILogger<UnreserveMoneyConsumer> _logger;

        public UnreserveMoneyConsumer(ILogger<UnreserveMoneyConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<UnreserveMoney> context)
        {
            _logger.LogInformation("[{consumerName}] Received money unreservation request for order {orderId}.", 
                nameof(UnreserveMoneyConsumer), context.Message.OrderId);

            await Task.Delay(1000);
            
            _logger.LogInformation("[{consumerName}] Unreserved {amount} money for order {orderId}.", 
                nameof(UnreserveMoneyConsumer), context.Message.Amount, context.Message.OrderId);

            await context.RespondAsync<MoneyUnreserved>(new
            {
                OrderId = context.Message.OrderId,
                Amount = context.Message.Amount
            });
        }
    }
}