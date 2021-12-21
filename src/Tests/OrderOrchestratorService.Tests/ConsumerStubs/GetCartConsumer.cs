using System.Threading.Tasks;
using CartService.Contracts;
using Contracts.Shared;
using MassTransit;

namespace OrderOrchestratorService.Tests.ConsumerStubs;

public class GetCartConsumer : IConsumer<GetCart>
{
    public async Task Consume(ConsumeContext<GetCart> context)
    {
        await context.RespondAsync<GetCartResponse>(new
        {

            OrderId = context.Message.OrderId,
            CartContent = new[] { 
                new CartPosition ()
                {
                    Amount = 5,
                    Name = "Food",
                    Price = 20
                } 
            },
            TotalPrice = 100
        });
    }
}