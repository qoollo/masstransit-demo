using System.Linq;
using System.Threading.Tasks;
using ApiService.Contracts.MonitoringApi;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using OrderOrchestratorService.Database;

namespace OrderOrchestratorService.Consumers
{
    public class GetAllOrdersStateConsumer : IConsumer<GetAllOrdersState>
    {
        private readonly StateMachinesDbContext _dbContext;

        public GetAllOrdersStateConsumer(StateMachinesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Consume(ConsumeContext<GetAllOrdersState> context)
        {
            var sagas = await _dbContext.OrderStates!.ToListAsync();

            var response = sagas.ToDictionary(s => s.CorrelationId, s => s.CurrentState);

            await context.RespondAsync<GetAllOrdersStateResponse>(new
            {
                States = response
            });
        }
    }
}