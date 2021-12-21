using HistoryService.Database.Models;
using HistoryService.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace HistoryService.Database.Repositories
{
    public class ArchivedOrderRepository : IArchivedOrderRepository
    {
        private readonly NpgSqlContext _context; 

        public ArchivedOrderRepository(NpgSqlContext context)
        {
            _context = context;
        }

        public async Task AddOrderAsync(Guid id,
            bool isConfirmed, 
            DateTimeOffset submitDate, 
            string manager, 
            DateTimeOffset? confirmDate, 
            DateTimeOffset? deliveredDate)
        {
            var archivedOrder = new ArchivedOrder(id, isConfirmed, submitDate, manager, confirmDate, deliveredDate);

            await _context.ArchivedOrders!
                .AddAsync(archivedOrder);

            await _context.SaveChangesAsync();
        }

        public async Task<ArchivedOrder> GetOrderAsync(Guid id)
        {
            var archivedOrder = await _context.ArchivedOrders!
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == id);

            return archivedOrder!;
        }
    }
}
