using CartService.Database.Repositories.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using CartService.Contracts;

namespace CartService.Consumers
{
    public class AddCartPositionConsumer : IConsumer<AddCartPosition>
    {

        private readonly ILogger<AddCartPositionConsumer> _logger;
        private readonly ICartRepository _cartRepository;
        private readonly IGoodRepository _goodRepository;
        private readonly ICartPositionRepository _cartPositionRepository;
        private readonly Random _random;

        public AddCartPositionConsumer(ILogger<AddCartPositionConsumer> logger,
            ICartRepository cartRepository,
            IGoodRepository goodRepository,
            ICartPositionRepository cartPositionRepository)
        {
            _random = new Random();
            
            _logger = logger;
            _cartRepository = cartRepository;
            _goodRepository = goodRepository;
            _cartPositionRepository = cartPositionRepository;
        }

        public async Task Consume(ConsumeContext<AddCartPosition> context)
        {
            
            var newCartPosition = context.Message;

            if (!await _cartRepository.CartExistsAsync(newCartPosition.OrderId))
            {
                await _cartRepository.AddCartAsync(newCartPosition.OrderId);
            }

            if (!await _goodRepository.GoodExistsAsync(newCartPosition.Name))
            {
                await _goodRepository.AddGoodAsync(Guid.NewGuid(), newCartPosition.Name, _random.Next(100, 150));
            }

            var cart = await _cartRepository.GetCartWithCartPositionsAsync(newCartPosition.OrderId);

            var cartPosition = cart.CartPositions!.FirstOrDefault(cp => cp.Good!.Name == newCartPosition.Name);

            if (cartPosition != null)
            {
                await _cartPositionRepository.UpdateCartPositionAsync(cartPosition.Id, cartPosition.CartId, cartPosition.GoodId, cartPosition.Amount + newCartPosition.Amount);
            }
            else
            {
                var good = await _goodRepository.GetGoodByNameAsync(newCartPosition.Name);

                await _cartPositionRepository.AddCartPositionAsync(Guid.NewGuid(), newCartPosition.OrderId, good.Id, newCartPosition.Amount);
            }

            

            //to-do: respond    
        }
    }
}
