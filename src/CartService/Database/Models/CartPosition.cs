using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Database.Models
{
    public class CartPosition
    {
        public Guid Id { get; set; }

        public Guid CartId { get; set; }

        public Cart? Cart { get; set; }

        public Guid GoodId { get; set; }

        public Good? Good { get; set; }

        public int Amount { get; set; }

        public CartPosition(Guid id,
            Guid cartId,
            Guid goodId,
            int amount)
        {
            Id = id;
            CartId = cartId;
            GoodId = goodId;
            Amount = amount;
        }
    }
}
