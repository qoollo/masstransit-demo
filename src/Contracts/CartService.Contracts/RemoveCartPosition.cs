using System;

namespace CartService.Contracts
{
    public interface RemoveCartPosition
    {
        Guid OrderId { get; }
        
        string Name { get; }

        int Amount { get; }
    }
}
