using System;
using System.Collections.Generic;

namespace ApiService.Contracts.MonitoringApi
{
    public interface GetAllOrdersStateResponse
    {
        Dictionary<Guid, int> States { get; set; }
    }
}