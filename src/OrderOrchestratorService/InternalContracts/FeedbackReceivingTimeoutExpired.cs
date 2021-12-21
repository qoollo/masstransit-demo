using System;

namespace OrderOrchestratorService.InternalContracts
{
    public interface FeedbackReceivingTimeoutExpired
    {
        public Guid OrderId { get; set; }
    }
}