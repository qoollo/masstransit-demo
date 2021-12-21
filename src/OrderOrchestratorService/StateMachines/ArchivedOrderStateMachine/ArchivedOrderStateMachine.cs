using System;
using System.Threading.Tasks;
using ApiService.Contracts.ManagerApi;
using Automatonymous;
using Automatonymous.Binders;
using CartService.Contracts;
using FeedbackService.Contracts;
using HistoryService.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrderOrchestratorService.Configurations;

namespace OrderOrchestratorService.StateMachines.ArchivedOrderStateMachine
{
    public class ArchivedOrderStateMachine : MassTransitStateMachine<ArchivedOrderState>
    {
#nullable disable
        private readonly ILogger<ArchivedOrderStateMachine> _logger;
        private readonly EndpointsConfiguration _endpointsConfiguration;

        public Event<GetArchivedOrder> ArchivedOrderRequested { get; set; }
        public Event InformationCollected { get; set; }

        public State AwaitingInformation { get; set; }

        public Request<ArchivedOrderState, GetOrderFromArchive, GetOrderFromArchiveResponse> ArchiveRequest { get; set; }
        public Request<ArchivedOrderState, GetOrderFeedback, GetOrderFeedbackResponse> OrderFeedbackRequest { get; set; }
        public Request<ArchivedOrderState, GetCart, GetCartResponse> CartRequest { get; set; }

        public ArchivedOrderStateMachine(IOptions<EndpointsConfiguration> settings,
            ILogger<ArchivedOrderStateMachine> logger)
        {
            _logger = logger;
            _endpointsConfiguration = settings.Value;

            InstanceState(x => x.CurrentState);

            BuildStateMachine();

            OnUnhandledEvent(HandleUnhandledEvent);

            Initially(WhenArchivedOrderRequested());

            During(AwaitingInformation,
                WhenCartRequestCompleted(),
                WhenArchiveRequestCompleted(),
                WhenOrderFeedbackRequestCompleted(),
                Ignore(ArchivedOrderRequested));

            DuringAny(
                When(InformationCollected)
                    .IfElse(context => context.ShouldBeResponded(),
                        responded => responded
                            .SendAsync(x => x.Instance.ResponseAddress, 
                                    x => x.CreateArchivedOrderResponse(), 
                                    (consumeContext, sendContext) => 
                                        sendContext.RequestId = consumeContext.Instance.RequestId),
                        published => published
                            .PublishAsync(x => x.CreateArchivedOrderResponse()))
                    .Finalize());

            CompositeEvent(() => InformationCollected,
                           x => x.InformationStatus,
                           CartRequest.Completed,
                           ArchiveRequest.Completed,
                           OrderFeedbackRequest.Completed);

            SetCompletedWhenFinalized();
        }

        private Task HandleUnhandledEvent(UnhandledEventContext<ArchivedOrderState> context)
        {
            _logger.LogDebug($"[{DateTime.Now}][SAGA] Ignored unhandled event: {context.Event.Name}");

            return Task.CompletedTask;
        }

        private void BuildStateMachine()
        {
            Event(() => ArchivedOrderRequested, x => x.CorrelateById(x => x.Message.OrderId));

            Request(() => ArchiveRequest, r =>
            {
                r.ServiceAddress = new Uri(_endpointsConfiguration.HistoryServiceAddress!);
            });

            Request(() => OrderFeedbackRequest, r =>
            {
                r.ServiceAddress = new Uri(_endpointsConfiguration.FeedbackServiceAddress!);
            });

            Request(() => CartRequest, r =>
            {
                r.ServiceAddress = new Uri(_endpointsConfiguration.CartServiceAddress!);
            });


        }

        private EventActivities<ArchivedOrderState> WhenArchivedOrderRequested()
        {
            return When(ArchivedOrderRequested)
                .TransitionTo(AwaitingInformation)
                .Then(x =>
                {
                    if (x.TryGetPayload(out SagaConsumeContext<ArchivedOrderState, GetArchivedOrder> payload))
                    {
                        x.Instance.RequestId = payload.RequestId;
                        x.Instance.ResponseAddress = payload.ResponseAddress;
                    }
                })
                .Request(CartRequest, x => x.Init<GetCart>(new
                {
                    OrderId = x.Instance.CorrelationId
                }))
                .Request(ArchiveRequest, x => x.Init<GetOrderFromArchive>(new
                {
                    OrderId = x.Instance.CorrelationId
                }))
                .Request(OrderFeedbackRequest, x => x.Init<GetOrderFeedback>(new
                {
                    OrderId = x.Instance.CorrelationId
                }));
        }

        private EventActivities<ArchivedOrderState> WhenCartRequestCompleted()
        {
            return When(CartRequest.Completed)
                .Then(x =>
                {
                    _logger.LogInformation("WhenOrderCartRequestCompleted");
                    x.Instance.Cart = x.Data.CartContent;
                    x.Instance.TotalPrice = x.Data.TotalPrice;
                });
        }

        private EventActivities<ArchivedOrderState> WhenArchiveRequestCompleted()
        {
            return When(ArchiveRequest.Completed)
                .Then(x =>
                {
                    _logger.LogInformation("WhenArchiveRequestCompleted");
                    x.Instance.ConfirmDate = x.Data.ConfirmDate;
                    x.Instance.SubmitDate = x.Data.SubmitDate;
                    x.Instance.IsConfirmed = x.Data.IsConfirmed;
                    x.Instance.Manager = x.Data.Manager;
                    x.Instance.DeliveredDate = x.Data.DeliveredDate;
                });
        }

        private EventActivities<ArchivedOrderState> WhenOrderFeedbackRequestCompleted()
        {
            return When(OrderFeedbackRequest.Completed)
                .Then(x =>
                {
                    _logger.LogInformation("WhenOrderFeedbackRequestCompleted");
                    x.Instance.FeedbackText = x.Data.Text;
                    x.Instance.FeedbackStars = x.Data.StarsAmount;
                });
        }
    }

    public static class ArchivedOrderStateMachineExtensions
    {
        public static Task<GetArchivedOrderResponse> CreateArchivedOrderResponse(this ConsumeEventContext<ArchivedOrderState> context)
        {
            return context.Init<GetArchivedOrderResponse>((new
            {
                OrderId = context.Instance.CorrelationId,
                Cart = context.Instance.Cart,
                TotalPrice = context.Instance.TotalPrice,
                IsConfirmed = context.Instance.IsConfirmed,
                SubmitDate = context.Instance.SubmitDate,
                Manager = context.Instance.Manager,
                ConfirmDate = context.Instance.ConfirmDate,
                DeliveredDate = context.Instance.DeliveredDate,
                FeedbackText = context.Instance.FeedbackText,
                FeedbackStars = context.Instance.FeedbackStars,
            }));
        }

        public static bool ShouldBeResponded(this BehaviorContext<ArchivedOrderState> context)
        {
            return context.Instance.RequestId.HasValue && context.Instance.ResponseAddress != null;
        }
    }
#nullable restore
}