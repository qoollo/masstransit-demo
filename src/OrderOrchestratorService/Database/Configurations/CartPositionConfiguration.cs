using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderOrchestratorService.Database.Models;

namespace OrderOrchestratorService.Database.Configurations
{
    public class CartPositionConfiguration : IEntityTypeConfiguration<CartPosition>
    {
        public void Configure(EntityTypeBuilder<CartPosition> builder)
        {
            builder.HasIndex(c => c.Id).IsUnique();
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .IsRequired()
                .ValueGeneratedNever();

            builder.Property(c => c.Name).IsRequired();
            builder.Property(c => c.Amount).IsRequired();
            builder.Property(c => c.Price).IsRequired();
        }
    }
}