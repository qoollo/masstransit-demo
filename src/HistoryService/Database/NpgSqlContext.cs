using HistoryService.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace HistoryService.Database
{
    public class NpgSqlContext : DbContext
    {
        public DbSet<ArchivedOrder>? ArchivedOrders { get; set; }

        public NpgSqlContext()
        {

        }

        public NpgSqlContext(DbContextOptions options) : base(options)
        {

        }

    }
}
