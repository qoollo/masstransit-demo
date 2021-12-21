using System;
using OrderOrchestratorService.StateMachines.OrderStateMachine;

namespace OrderOrchestratorService.Database.Models
{
    public class CartPosition
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public OrderState? OrderState { get; set; }
        public string? Name { get; set; }
        public int Amount { get; set; }
        public int Price { get; set; }

        public CartPosition()
        {
            
        }

        public CartPosition(Guid id, 
            Guid orderId, 
            string name, 
            int amount, 
            int price)
        {
            Id = id;
            OrderId = orderId;
            Name = name;
            Amount = amount;
            Price = price;
        }
    }
}