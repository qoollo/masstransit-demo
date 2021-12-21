using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Database.Models
{
    public class Good
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public int Price { get; set; }

        public Good(Guid id,
            string? name,
            int price)
        {
            Id = id;
            Name = name;
            Price = price;
        }
    }
}
