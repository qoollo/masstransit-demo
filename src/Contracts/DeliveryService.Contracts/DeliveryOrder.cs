using System;
using System.Collections.Generic;
using Contracts.Shared;

namespace DeliveryService.Contracts
{
    public interface DeliveryOrder
    {
        public Guid OrderId { get; set; }

        public List<CartPosition> Cart { get; set; }
    }
}
