using CartService.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Database.Configurations
{
    internal class CartConfiguration : IEntityTypeConfiguration<Cart>
    {
        public void Configure(EntityTypeBuilder<Cart> builder)
        {
            builder.HasIndex(c => c.Id).IsUnique();
            builder.HasKey(c => c.Id);

            builder.HasMany(c => c.CartPositions)
                .WithOne(cp => cp.Cart!)
                .HasForeignKey(cp => cp.CartId);
        }
    }
}
