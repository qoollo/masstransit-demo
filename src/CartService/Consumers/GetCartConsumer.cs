using CartService.Database.Models;
using CartService.Database.Repositories.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CartService.Contracts;
using DbCartPosition = CartService.Database.Models.CartPosition;
using DtoCartPosition = Contracts.Shared.CartPosition;

namespace CartService.Consumers
{
    public class GetCartConsumer : IConsumer<GetCart>
    {

        private readonly ILogger<GetCartConsumer> _logger;
        private readonly ICartRepository _cartRepository;

        public GetCartConsumer(ILogger<GetCartConsumer> logger,
            ICartRepository cartRepository)
        {
            _logger = logger;
            _cartRepository = cartRepository;
        }

        public async Task Consume(ConsumeContext<GetCart> context)
        {
            if (context.RequestId != null)
            {
                var message = context.Message;

                var cart = await _cartRepository.GetCartWithCartPositionsAsync(message.OrderId);

                await context.RespondAsync<GetCartResponse>(new
                {
                    OrderId = message.OrderId,
                    CartContent = cart.CartPositions!.Select(c => ConvertToContractCartPosition(c)).ToList(),
                    TotalPrice = CountPrice(cart.CartPositions!.ToList())
                }) ;
            }
        }

        private int CountPrice(List<DbCartPosition> cartPositions)
        {
            int totalPrice = 0;

            foreach (var position in cartPositions)
            {
                totalPrice += position.Good!.Price * position.Amount;
            }

            return totalPrice;
        }

        //private int CountPrice(List<DbCartPosition> cartPositions)
        //{
        //    return cartPositions.Aggregate(0, (total, next) =>
        //    {
        //        total += next.Amount * next.Good!.Price;
        //        return total;
        //    });

        //}

        private DtoCartPosition ConvertToContractCartPosition(DbCartPosition dbCartPosition)
        {
            return new DtoCartPosition()
            {
                Amount = dbCartPosition.Amount,
                Name = dbCartPosition.Good!.Name,
                Price = dbCartPosition.Good!.Price
            };
        }
    }
}
