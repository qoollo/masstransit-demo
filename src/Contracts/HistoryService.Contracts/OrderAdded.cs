using System;

namespace HistoryService.Contracts
{
    public interface OrderAdded
    {
        public Guid OrderId { get; set; }
    }
}
