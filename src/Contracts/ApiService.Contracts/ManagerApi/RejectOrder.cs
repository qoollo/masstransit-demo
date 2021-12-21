using System;

namespace ApiService.Contracts.ManagerApi
{
    public interface RejectOrder
    {
        public Guid OrderId { get; set; }

        public string RejectManager { get; set; }

        public string Reason { get; set; }
    }
}
