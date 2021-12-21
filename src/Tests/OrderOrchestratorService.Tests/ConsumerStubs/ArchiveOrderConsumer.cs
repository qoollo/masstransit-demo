using HistoryService.Contracts;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderOrchestratorService.Tests.ConsumerStubs
{
    internal class ArchiveOrderConsumer : IConsumer<ArchiveOrder>
    {
        public async Task Consume(ConsumeContext<ArchiveOrder> context)
        {
            await context.RespondAsync<OrderAdded>(new
            {
                OrderId = context.Message.OrderId
            });
        }
    }
}
