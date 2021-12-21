using System;

namespace PaymentService.Contracts
{
    public interface UnreserveMoney
    {
        public Guid OrderId { get; set; }

        public int Amount { get; set; }
    }
}
