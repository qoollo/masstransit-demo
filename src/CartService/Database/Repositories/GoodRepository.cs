using CartService.Database.Models;
using CartService.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Database.Repositories
{
    public class GoodRepository : IGoodRepository
    {

        private readonly NpgSqlContext _dbContext;

        public GoodRepository(NpgSqlContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddGoodAsync(Guid id,
            string? name,
            int price)
        {
            var good = new Good(id, name, price);

            await _dbContext.Goods!.AddAsync(good);
            
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Good> GetGoodByNameAsync(string name)
        {
            var good = await _dbContext.Goods!
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.Name == name);

            return good!;
        }

        public async Task<bool> GoodExistsAsync(string name)
        {
            var good = await _dbContext.Goods!
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.Name == name);

            return good != null;
        }
    }
}
