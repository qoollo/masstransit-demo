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
    public class CartPositionRepository : ICartPositionRepository
    {
        private readonly NpgSqlContext _dbContext;

        public CartPositionRepository(NpgSqlContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddCartPositionAsync(Guid id, Guid cartId, Guid goodId, int amount)
        {
            var cartPosition = new CartPosition(id, cartId, goodId, amount);

            await _dbContext.CartPositions!.AddAsync(cartPosition);

            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> CartPositionExistsForCartAsync(Guid cartId, string name)
        {
            var cart = await _dbContext.CartPositions!
               .Include(cp => cp.Good)
               .AsNoTracking()
               .Where(cp => cp.CartId == cartId)
               .FirstOrDefaultAsync(cp => cp.Good!.Name == name);

            return cart != null;
        }

        public async Task RemoveCartPositionAsync(Guid id)
        {
            var cartPosition = await _dbContext.CartPositions!.FirstOrDefaultAsync(cp => cp.Id == id);

            _dbContext.Remove(cartPosition!);

            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateCartPositionAsync(Guid id, Guid cartId, Guid goodId, int amount)
        {
            var cartPosition = await _dbContext.CartPositions!.FirstOrDefaultAsync(cp => cp.Id == id);

            if (cartPosition != null)
            {
                cartPosition.Id = id;
                cartPosition.CartId = cartId;
                cartPosition.GoodId = goodId;
                cartPosition.Amount = amount;
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
