using System;
using System.Collections.Generic;
using Automatonymous;
using Contracts.Shared;

namespace OrderOrchestratorService.StateMachines.ArchivedOrderStateMachine
{
#nullable disable
    public class ArchivedOrderState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public int CurrentState { get; set; }
        public int InformationStatus { get; set; }
        public Guid? RequestId { get; set; }
        public Uri ResponseAddress { get; set; }

        public List<CartPosition> Cart { get; set; }

        public int TotalPrice { get; set; }

        public bool IsConfirmed { get; set; }

        public DateTimeOffset SubmitDate { get; set; }

        public string Manager { get; set; }

        public DateTimeOffset? ConfirmDate { get; set; }

        public DateTimeOffset? DeliveredDate { get; set; }

        public string FeedbackText { get; set; }
        public int FeedbackStars { get; set; }
    }
#nullable restore
}