using MassTransit.EntityFrameworkCoreIntegration.Mappings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderOrchestratorService.StateMachines.OrderStateMachine;

namespace OrderOrchestratorService.Database.Configurations
{
    public class OrderStateMap : SagaClassMap<OrderState>
    {
        protected override void Configure(EntityTypeBuilder<OrderState> entity, ModelBuilder model)
        {
            entity.HasIndex(c => c.CorrelationId).IsUnique();
            entity.HasKey(c => c.CorrelationId);

            entity.Property(x => x.RowVersion).IsRowVersion();

            entity.HasMany(o => o.Cart)
                .WithOne(c => c.OrderState!)
                .HasForeignKey(c => c.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Navigation(c => c.Cart).AutoInclude();
        }
    }
}