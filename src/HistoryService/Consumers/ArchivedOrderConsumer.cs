using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using HistoryService.Contracts;
using HistoryService.Database.Repositories.Interfaces;

namespace HistoryService.Consumers
{
    public class ArchivedOrderConsumer : IConsumer<ArchiveOrder>
    {
        private readonly ILogger<ArchivedOrderConsumer> _logger;
        private readonly IArchivedOrderRepository _archivedOrderRepository;

        public ArchivedOrderConsumer(ILogger<ArchivedOrderConsumer> logger,
            IArchivedOrderRepository archivedOrderRepository)
        {
            _archivedOrderRepository = archivedOrderRepository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ArchiveOrder> context)
        {
            _logger.LogInformation("[{consumerName}] Received order archive request for order {orderId}.",
            nameof(ArchivedOrderConsumer), context.Message.OrderId);

            var message = context.Message;

            await _archivedOrderRepository.AddOrderAsync(message.OrderId, 
                message.IsConfirmed, 
                message.SubmitDate, 
                message.Manager, 
                message.ConfirmDate, 
                message.DeliveredDate);

            if (context.RequestId != null)
            {
                await context.RespondAsync<OrderAdded>(new
                {
                    OrderId = message.OrderId
                });
            }
        }
    }
}
