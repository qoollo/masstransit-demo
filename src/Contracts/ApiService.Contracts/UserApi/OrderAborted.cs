using System;

namespace ApiService.Contracts.UserApi
{
    public interface OrderAborted
    {
        public Guid OrderId { get; set; }
    }
}
