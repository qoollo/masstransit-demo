using System.Threading.Tasks;
using ApiService.Contracts.ManagerApi;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace ApiService.Consumers
{
    public class GetArchivedOrderResponseConsumer : IConsumer<GetArchivedOrderResponse>
    {
        private readonly ILogger<GetArchivedOrderResponseConsumer> _logger;

        public GetArchivedOrderResponseConsumer(ILogger<GetArchivedOrderResponseConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<GetArchivedOrderResponse> context)
        {
            _logger.LogInformation($"[ArchivedOrder] {context.Message.OrderId} " +
                $"{context.Message.FeedbackText} {context.Message.FeedbackStars}" +
                $"{context.Message.ConfirmDate} { context.Message.Cart.Count}");
            return Task.CompletedTask;
        }
    }
}
