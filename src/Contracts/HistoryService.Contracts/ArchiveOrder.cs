using System;
using System.Collections.Generic;
using Contracts.Shared;

namespace HistoryService.Contracts
{
    public interface ArchiveOrder
    {
        public Guid OrderId { get; set; }

        public List<CartPosition> Cart { get; set; }

        public int TotalPrice { get; set; }

        public bool IsConfirmed { get; set; }

        public DateTimeOffset SubmitDate { get; set; }

        public string Manager { get; set; }

        public DateTimeOffset? ConfirmDate { get; set; }

        public DateTimeOffset? DeliveredDate { get; set; }
    }
}
