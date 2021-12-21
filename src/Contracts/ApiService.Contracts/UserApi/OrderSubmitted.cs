using System;

namespace ApiService.Contracts.UserApi
{
    public interface OrderSubmitted
    {
        public Guid OrderId { get; set; }

        public string UserName { get; set; }

    }
}
