using System;

namespace PaymentService.Contracts
{
    public interface MoneyUnreserved
    {
        Guid OrderId { get; }
        int Amount { get; }
    }
}