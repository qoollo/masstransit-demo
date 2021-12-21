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
    public class GoodConfiguration : IEntityTypeConfiguration<Good>
    {
        public void Configure(EntityTypeBuilder<Good> builder)
        {
            builder.HasKey(g => g.Id);
            builder.HasIndex(g => g.Id).IsUnique();

            builder.Property(g => g.Name).IsRequired();
            builder.Property(g => g.Price).IsRequired();
        }
    }
}
