using System.Collections.Generic;
using MassTransit.EntityFrameworkCoreIntegration;
using MassTransit.EntityFrameworkCoreIntegration.Mappings;
using Microsoft.EntityFrameworkCore;
using OrderOrchestratorService.Database.Configurations;
using OrderOrchestratorService.Database.Models;
using OrderOrchestratorService.StateMachines.OrderStateMachine;

namespace OrderOrchestratorService.Database
{
    public class StateMachinesDbContext : SagaDbContext
    {
        public DbSet<OrderState>? OrderStates { get; set; }
        public DbSet<CartPosition>? CartPositions { get; set; }

        public StateMachinesDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CartPositionConfiguration());
            base.OnModelCreating(modelBuilder);
        }

        protected override IEnumerable<ISagaClassMap> Configurations
        {
            get
            {
                yield return new OrderStateMap();
            }

        }
    }
}