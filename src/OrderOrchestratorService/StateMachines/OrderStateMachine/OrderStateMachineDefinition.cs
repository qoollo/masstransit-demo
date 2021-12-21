using GreenPipes;
using MassTransit;
using MassTransit.Definition;

namespace OrderOrchestratorService.StateMachines.OrderStateMachine
{
#nullable restore
    public class OrderStateMachineDefinition : SagaDefinition<OrderState>
    {
        public OrderStateMachineDefinition()
        {

        }

        protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator,
            ISagaConfigurator<OrderState> sagaConfigurator)
        {
            endpointConfigurator.UseMessageRetry(r => r.Intervals(50, 100, 500, 1000));
            endpointConfigurator.UseInMemoryOutbox();
        }
    }
}
