using MassTransit;
using PaymentService.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderOrchestratorService.Tests.ConsumerStubs
{
    public class ReserveMoneyConsumer : IConsumer<ReserveMoney>
    {
        public async Task Consume(ConsumeContext<ReserveMoney> context)
        {
            await Task.Delay(1000);

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
