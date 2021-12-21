using System;
using System.Collections.Generic;
using Contracts.Shared;

namespace ApiService.Contracts.ManagerApi
{
    public interface NewOrderConfirmationRequested
    {
        public Guid OrderId { get; set; }

        public List<CartPosition> Cart { get; set; }

        public int TotalPrice { get; set; }
        
    }
}
