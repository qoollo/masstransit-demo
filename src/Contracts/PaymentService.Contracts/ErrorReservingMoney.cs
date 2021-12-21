using System;

namespace PaymentService.Contracts
{
    public interface ErrorReservingMoney
    {
        public Guid OrderId { get; }

        public string Reason { get; }
    }
}
