using System;

namespace HistoryService.Database.Models
{
    public class ArchivedOrder
    {
        public Guid Id { get; set; }

        public bool IsConfirmed { get; set; }

        public DateTimeOffset SubmitDate { get; set; }

        public string? Manager { get; set; }

        public DateTimeOffset? ConfirmDate { get; set; }

        public DateTimeOffset? DeliveredDate { get; set; }

        public ArchivedOrder(Guid id,
            bool isConfirmed,
            DateTimeOffset submitDate,
            string manager,
            DateTimeOffset? confirmDate,
            DateTimeOffset? deliveredDate)
        {
            Id = id;
            IsConfirmed = isConfirmed;
            SubmitDate = submitDate;
            Manager = manager;
            ConfirmDate = confirmDate;
            DeliveredDate = deliveredDate;
        }
    }
}
