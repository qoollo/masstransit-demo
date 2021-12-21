using System;

namespace FeedbackService.Contracts
{
    public interface FeedbackAdded
    {
        public Guid OrderId { get; set; }
    }
}
