using System;

namespace ApiService.Contracts.MonitoringApi
{
    public interface GetOrderState
    {
        Guid OrderId { get; set; }
    }
}