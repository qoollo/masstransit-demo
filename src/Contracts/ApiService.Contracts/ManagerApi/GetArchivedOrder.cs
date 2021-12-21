using System;

namespace ApiService.Contracts.ManagerApi
{
    public interface GetArchivedOrder
    {
        public Guid OrderId { get; set; }
    }   
}