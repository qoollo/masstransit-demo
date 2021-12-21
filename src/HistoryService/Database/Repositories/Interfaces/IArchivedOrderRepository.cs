using HistoryService.Database.Models;
using System;
using System.Threading.Tasks;

namespace HistoryService.Database.Repositories.Interfaces
{
    public interface IArchivedOrderRepository
    {
        public Task AddOrderAsync(Guid id,
            bool isConfirmed,
            DateTimeOffset submitDate,
            string manager,
            DateTimeOffset? confirmDate,
            DateTimeOffset? deliveredDate);

        public Task<ArchivedOrder> GetOrderAsync(Guid id);

    }
}
