using System;

namespace ApiService.Contracts.ManagerApi
{
    public interface ConfirmOrder
    {
        public Guid OrderId { get; set; }

        public string ConfirmManager { get; set; }
    }
}
