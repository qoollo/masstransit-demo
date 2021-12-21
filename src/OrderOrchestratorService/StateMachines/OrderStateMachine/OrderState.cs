using System;
using System.Collections.Generic;
using Automatonymous;
using OrderOrchestratorService.Database.Models;

namespace OrderOrchestratorService.StateMachines.OrderStateMachine
{
#nullable disable
    public class OrderState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        
        public int CurrentState { get; set; }

        public DateTimeOffset? SubmitDate { get; set; }

        public int TotalPrice { get; set; }

        public List<CartPosition> Cart { get; set; }

        public string Manager { get; set; }

        public bool IsConfirmed { get; set; }

        public DateTimeOffset? ConfirmationDate { get; set; }

        public DateTimeOffset? DeliveryDate { get; set; }

        public byte[] RowVersion { get; set; }
        public Guid? FeedbackReceivingTimeoutToken { get; set; }
    }
#nullable restore
}
