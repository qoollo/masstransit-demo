using System;

namespace ApiService.Contracts.UserApi
{
    public interface FeedbackRequested
    {
        public Guid OrderId { get; set; }
    }
}
