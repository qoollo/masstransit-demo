using System;

namespace CartService.Contracts
{
    public interface AddCartPosition
    {
        Guid OrderId { get; }

        string Name { get; }

        int Amount { get; }
    }
}
