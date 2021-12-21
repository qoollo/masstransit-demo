using CartService.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Database.Repositories.Interfaces
{
    public interface ICartRepository
    {
        public Task AddCartAsync(Guid id);

        public Task<bool> CartExistsAsync(Guid id);

        public Task<Cart> GetCartWithCartPositionsAsync(Guid id);
    }
}
