using System;

namespace CartService.Contracts
{
    public interface GetCart
    {
        public Guid OrderId { get; set; }
    }
}
