using System;

namespace ApiService.Contracts.UserApi
{
    public interface OrderRejected
    {
        public Guid OrderId { get; set; }
        public string Reason { get; set; }
    }
}