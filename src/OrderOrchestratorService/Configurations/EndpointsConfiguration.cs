namespace OrderOrchestratorService.Configurations
{
    public class EndpointsConfiguration
    {
        public string? OrderStateMachineAddress { get; set; }
        public string? ArchiveOrderStateMachineAddress { get; set; }
        public string? CartServiceAddress { get; set; }
        public string? PaymentServiceAddress { get; set; }
        public string? MessageSchedulerAddress { get; set; }
        public string? DeliveryServiceAddress { get; set; }
        public string? FeedbackServiceAddress { get; set; }
        public string? HistoryServiceAddress { get; set; }
    }
}
