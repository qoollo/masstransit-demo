using System;
using System.Threading.Tasks;
using HistoryService.Contracts;
using HistoryService.Database.Repositories.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace HistoryService.Consumers
{
    public class GetOrderFromArchiveConsumer : IConsumer<GetOrderFromArchive>
    {
        private readonly ILogger<GetOrderFromArchiveConsumer> _logger;
        private readonly IArchivedOrderRepository _archivedOrderRepository;

        public GetOrderFromArchiveConsumer(ILogger<GetOrderFromArchiveConsumer> logger, 
            IArchivedOrderRepository archivedOrderRepository)
        {
            _logger = logger;
            _archivedOrderRepository = archivedOrderRepository;
        }

        public async Task Consume(ConsumeContext<GetOrderFromArchive> context)
        {
            var archiveOrder = await _archivedOrderRepository.GetOrderAsync(context.Message.OrderId);

            await context.RespondAsync<GetOrderFromArchiveResponse>(new
            {

                OrderId = archiveOrder.Id,
                IsConfirmed = archiveOrder.IsConfirmed,
                SubmitDate = archiveOrder.SubmitDate,
                Manager = archiveOrder.Manager,
                ConfirmDate = archiveOrder.ConfirmDate,
                DeliveredDate = archiveOrder.DeliveredDate
            });
        }
    }
}