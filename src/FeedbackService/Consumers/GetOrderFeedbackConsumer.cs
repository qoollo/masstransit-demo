using System.Threading.Tasks;
using FeedbackService.Contracts;
using FeedbackService.Database.Repositories.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace FeedbackService.Consumers
{
    public class GetOrderFeedbackConsumer : IConsumer<GetOrderFeedback>
    {
        private readonly ILogger<GetOrderFeedbackConsumer> _logger;
        private readonly IFeedbackRepository _feedbackRepository;

        public GetOrderFeedbackConsumer(ILogger<GetOrderFeedbackConsumer> logger, 
            IFeedbackRepository feedbackRepository)
        {
            _logger = logger;
            _feedbackRepository = feedbackRepository;
        }

        public async Task Consume(ConsumeContext<GetOrderFeedback> context)
        {
            var feedback = await _feedbackRepository.FindFeedbackAsync(context.Message.OrderId);

            await context.RespondAsync<GetOrderFeedbackResponse>(new
            {
                OrderId = context.Message.OrderId,
                Text = feedback?.Text,
                StarsAmount = feedback?.StarsAmount
            });
        }
    }
}