using System;

namespace PaymentService.Contracts
{
    public interface ReserveMoney
    {
        public Guid OrderId { get; set; }

        public int Amount { get; set; }
    }
}
