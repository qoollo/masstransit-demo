using System;

namespace ApiService.Contracts.UserApi
{
    public interface AbortOrder
    {
        public Guid OrderId { get; set; }
    }
}
