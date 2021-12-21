using System.Threading.Tasks;
using ApiService.Contracts.MonitoringApi;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using OrderOrchestratorService.Database;

namespace OrderOrchestratorService.Consumers
{
    public class GetOrderStateConsumer : IConsumer<GetOrderState>
    {
        private readonly StateMachinesDbContext _dbContext;

        public GetOrderStateConsumer(StateMachinesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Consume(ConsumeContext<GetOrderState> context)
        {
            var orderId = context.Message.OrderId;
            var saga = await _dbContext.OrderStates!.FirstOrDefaultAsync(o => o.CorrelationId == orderId);

            if (saga != null)
            {
                await context.RespondAsync<GetOrderStateResponse>(new
                {
                    OrderId = orderId,
                    State = saga.CurrentState
                });
            }
        }
    }
}