using System;

namespace ApiService.Contracts.MonitoringApi
{
    public interface GetOrderStateResponse
    {
        public Guid OrderId { get; set; }
        public int State { get; set; }
    }
}