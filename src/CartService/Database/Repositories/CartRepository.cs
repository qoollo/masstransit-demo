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
    public class CartRepository : ICartRepository
    {

        private readonly NpgSqlContext _dbContext;

        public CartRepository(NpgSqlContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddCartAsync(Guid id)
        {
            var cart = new Cart(id);

            await _dbContext.Carts!.AddAsync(cart);

            await _dbContext.SaveChangesAsync();
        }

        public async Task<Cart> GetCartWithCartPositionsAsync(Guid id)
        {
            var cart = await _dbContext.Carts!
                .Include(c => c.CartPositions!)
                    .ThenInclude(cp => cp.Good)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            return cart!;
        }

        public async Task<bool> CartExistsAsync(Guid id)
        {
            var cart = await _dbContext.Carts!
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            return cart != null;
        }
    }
}
