using System;

namespace FeedbackService.Contracts
{
    public interface GetOrderFeedback
    {
        public Guid OrderId { get; set; }
    }
}