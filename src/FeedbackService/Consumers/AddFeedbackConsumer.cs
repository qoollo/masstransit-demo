using FeedbackService.Database.Repositories.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FeedbackService.Contracts;

namespace FeedbackService.Consumers
{
    public class AddFeedbackConsumer : IConsumer<AddFeedback>
    {
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly ILogger<AddFeedbackConsumer> _logger;


        public AddFeedbackConsumer(ILogger<AddFeedbackConsumer> logger,
            IFeedbackRepository feedbackRepository)
        {
            _logger = logger;
            _feedbackRepository = feedbackRepository;
        }

        public async Task Consume(ConsumeContext<AddFeedback> context)
        {
            
            _logger.LogInformation("[{consumerName}] Received feedback add request for order {orderId}.",
            nameof(AddFeedbackConsumer), context.Message.OrderId);

            var message = context.Message;

            await _feedbackRepository.AddFeedbackAsync(message.OrderId, message.Text, message.StarsAmount);

            if (context.RequestId != null)
            {
                await context.RespondAsync<FeedbackAdded>(new
                {
                    OrderId = message.OrderId
                });
            }
        }
    }
}
