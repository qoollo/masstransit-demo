using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Database.Models
{
    public class Cart
    {
        public Guid Id { get; set; }

        public ICollection<CartPosition>? CartPositions { get; set; }

        public Cart(Guid id)
        {
            Id = id;
        }
    }
}
