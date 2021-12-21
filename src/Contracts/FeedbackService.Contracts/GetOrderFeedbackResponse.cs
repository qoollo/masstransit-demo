using System;

namespace FeedbackService.Contracts
{
    public interface GetOrderFeedbackResponse
    {
        public Guid OrderId { get; set; }

        public string Text { get; set; }
        public int StarsAmount { get; set; }
    }
}