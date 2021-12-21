using System.Threading.Tasks;
using ApiService.Contracts.ManagerApi;
using CartService.Contracts;
using FeedbackService.Contracts;
using HistoryService.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace OrderOrchestratorService.Consumers
{
    public class GetArchivedOrderConsumer : IConsumer<GetArchivedOrder>
    {
        private readonly ILogger<GetArchivedOrderConsumer> _logger;
        private readonly IRequestClient<GetCart> _cartRequestClient;
        private readonly IRequestClient<GetOrderFromArchive> _archiveRequestClient;
        private readonly IRequestClient<GetOrderFeedback> _feedbackRequestClient;

        public GetArchivedOrderConsumer(ILogger<GetArchivedOrderConsumer> logger, 
            IRequestClient<GetCart> cartRequestClient, 
            IRequestClient<GetOrderFromArchive> archiveRequestClient, 
            IRequestClient<GetOrderFeedback> feedbackRequestClient)
        {
            _logger = logger;
            _cartRequestClient = cartRequestClient;
            _archiveRequestClient = archiveRequestClient;
            _feedbackRequestClient = feedbackRequestClient;
        }

        public async Task Consume(ConsumeContext<GetArchivedOrder> context)
        {
            var orderId = context.Message.OrderId;

            var cart = (await _cartRequestClient.GetResponse<GetCartResponse>(new
            {
                OrderId = orderId
            })).Message;

            var archive = (await _archiveRequestClient.GetResponse<GetOrderFromArchiveResponse>(new
            {
                OrderId = orderId
            })).Message;

            var feedback = (await _feedbackRequestClient.GetResponse<GetOrderFeedbackResponse>(new
            {
                OrderId = orderId
            })).Message;

            await context.RespondAsync<GetArchivedOrderResponse>(new
            {

                OrderId = orderId,
                Cart = cart.CartContent,
                TotalPrice = cart.TotalPrice,
                IsConfirmed = archive.IsConfirmed,
                SubmitDate = archive.SubmitDate,
                Manager = archive.Manager,
                ConfirmDate = archive.ConfirmDate,
                DeliveredDate = archive.DeliveredDate,
                FeedbackText = feedback.Text,
                FeedbackStars = feedback.StarsAmount
            });
        }
    }
}