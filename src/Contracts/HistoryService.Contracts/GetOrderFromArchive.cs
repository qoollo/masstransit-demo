using System;

namespace HistoryService.Contracts
{
    public interface GetOrderFromArchive
    {
        public Guid OrderId { get; set; }
    }
}