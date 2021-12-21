using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using PaymentService.Contracts;

namespace PaymentService.Consumers
{
    public class ReserveMoneyConsumer : IConsumer<ReserveMoney>
    {
        private readonly ILogger<ReserveMoneyConsumer> _logger;

        public ReserveMoneyConsumer(ILogger<ReserveMoneyConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ReserveMoney> context)
        {
            //throw new System.Exception("Hello!");

            _logger.LogInformation("[{consumerName}] Received money reservation request for order {orderId}.",
                nameof(ReserveMoneyConsumer), context.Message.OrderId);

            await Task.Delay(1000);

            _logger.LogInformation("[{consumerName}] Reserved {amount} money for order {orderId}.",
                nameof(ReserveMoneyConsumer), context.Message.Amount, context.Message.OrderId);

            if (context.Message.Amount <= 0)
            {
                await context.RespondAsync<ErrorReservingMoney>(new
                {
                    OrderId = context.Message.OrderId,
                    Reason = "Can not reserve less than 1"
                });
            }
            else
            {
                await context.RespondAsync<MoneyReserved>(new
                {
                    OrderId = context.Message.OrderId,
                    Amount = context.Message.Amount
                });
            }
        }
    }
}