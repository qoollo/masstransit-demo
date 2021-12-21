using System;
using System.Collections.Generic;
using System.Linq;
using DtoCartPosition = Contracts.Shared.CartPosition;
using DbCartPosition = OrderOrchestratorService.Database.Models.CartPosition;

namespace OrderOrchestratorService.Database.Converters
{
    public static class FromDtoCartPositionToDbConverter
    {
        public static DbCartPosition Convert(DtoCartPosition dtoCartPosition, Guid orderId)
        {
            return new DbCartPosition(Guid.NewGuid(), orderId, dtoCartPosition.Name, dtoCartPosition.Amount,
                dtoCartPosition.Price);
        }

        public static DtoCartPosition ConvertBack(DbCartPosition dbCartPosition)
        {
            return new DtoCartPosition()
            {
                Name = dbCartPosition.Name,
                Amount = dbCartPosition.Amount,
                Price = dbCartPosition.Price
            };
        }

        public static List<DbCartPosition> ConvertMany(List<DtoCartPosition> dtoCartPositions, Guid orderId)
        {
            return dtoCartPositions.Select(c => Convert(c, orderId)).ToList();
        }

        public static List<DtoCartPosition> ConvertBackMany(List<DbCartPosition> dbCartPositions)
        {
            return dbCartPositions.Select(c => ConvertBack(c)).ToList();
        }
    }
}