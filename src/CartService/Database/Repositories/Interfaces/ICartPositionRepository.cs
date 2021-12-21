using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Database.Repositories.Interfaces
{
    public interface ICartPositionRepository
    {
        public Task AddCartPositionAsync(Guid id,
            Guid cartId,
            Guid goodId,
            int amount);

        public Task UpdateCartPositionAsync(Guid id,
            Guid cartId,
            Guid goodId,
            int amount);

        public Task<bool> CartPositionExistsForCartAsync(Guid cartId, string name);

        public Task RemoveCartPositionAsync(Guid id);
    }
}
