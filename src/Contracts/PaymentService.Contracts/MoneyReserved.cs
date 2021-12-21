using System;

namespace PaymentService.Contracts
{
    public interface MoneyReserved
    {
        Guid OrderId { get; }
        int Amount { get; }
    }
}