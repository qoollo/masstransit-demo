using System;
using System.Collections.Generic;
using Contracts.Shared;

namespace CartService.Contracts
{
    public interface GetCartResponse
    {
        public Guid OrderId { get; set; }

        public List<CartPosition> CartContent { get; set; }

        public int TotalPrice { get; set; }
    }
}
