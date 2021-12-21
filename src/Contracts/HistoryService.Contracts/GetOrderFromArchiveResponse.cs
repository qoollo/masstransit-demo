using System;

namespace HistoryService.Contracts
{
    public interface GetOrderFromArchiveResponse
    {
        public Guid OrderId { get; set; }

        public bool IsConfirmed { get; set; }

        public DateTimeOffset SubmitDate { get; set; }

        public string Manager { get; set; }

        public DateTimeOffset? ConfirmDate { get; set; }

        public DateTimeOffset? DeliveredDate { get; set; }
    }
}