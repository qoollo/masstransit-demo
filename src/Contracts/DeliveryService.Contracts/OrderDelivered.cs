using System;

namespace DeliveryService.Contracts
{
    public interface OrderDelivered
    {
        public Guid OrderId { get; set; }
    }
}
