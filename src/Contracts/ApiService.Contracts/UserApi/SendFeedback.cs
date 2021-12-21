using System;

namespace ApiService.Contracts.UserApi
{
    public interface SendFeedback
    {
        public Guid OrderId { get; set; }

        public string Text { get; set; }

        public int StarsAmount { get; set; }
    }
}
