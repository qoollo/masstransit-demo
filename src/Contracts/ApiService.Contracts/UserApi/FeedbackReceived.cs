using System;

namespace ApiService.Contracts.UserApi
{
    public interface FeedbackReceived
    {
        Guid OrderId { get; }

        string Text { get; }

        public int StarsAmount { get; }
    }
}
