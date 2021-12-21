using System;
using System.Threading.Tasks;
using ApiService.Contracts.ManagerApi;
using ApiService.Contracts.UserApi;
using Automatonymous;
using Automatonymous.Binders;
using CartService.Contracts;
using DeliveryService.Contracts;
using FeedbackService.Contracts;
using HistoryService.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrderOrchestratorService.Configurations;
using OrderOrchestratorService.Database.Converters;
using OrderOrchestratorService.InternalContracts;
using PaymentService.Contracts;

namespace OrderOrchestratorService.StateMachines.OrderStateMachine
{
#nullable disable
    public class OrderStateMachine : MassTransitStateMachine<OrderState>
    {

        private readonly EndpointsConfiguration _settings;
        private readonly ILogger<OrderStateMachine> _logger;

        public State AwaitingConfirmation { get; private set; }
        public State AwaitingDelivery { get; private set; }
        public State AwaitingFeedback { get; private set; }

        public Event<OrderSubmitted> OrderSubmitted { get; private set; }
        public Event<ConfirmOrder> OrderConfirmed { get; private set; }
        public Event<RejectOrder> OrderRejected { get; private set; }
        public Event<OrderDelivered> OrderDelivered { get; private set; }
        public Event<FeedbackReceived> ReceivedFeedback { get; private set; }
        public Event<AbortOrder> OrderAborted { get; private set; } 

        public Request<OrderState, GetCart, GetCartResponse> CartRequest { get; private set; }
        public Request<OrderState, ReserveMoney, MoneyReserved> MoneyReservationRequest { get; private set; }
        public Request<OrderState, UnreserveMoney, MoneyUnreserved> MoneyUnreservationRequest { get; private set; }
        public Request<OrderState, AddFeedback, FeedbackAdded> AddFeedbackRequest { get; private set; }
        public Request<OrderState, ArchiveOrder, OrderAdded> ArchiveOrderRequest { get; private set; }

        public Schedule<OrderState, FeedbackReceivingTimeoutExpired> FeedbackReceivingTimeout { get; set; }

        public OrderStateMachine(IOptions<EndpointsConfiguration> settings, ILogger<OrderStateMachine> logger)
        {
            _settings = settings.Value;
            _logger = logger;

            InstanceState(x => x.CurrentState);

            BuildStateMachine();

            OnUnhandledEvent(HandleUnhandledEvent);
            
            Initially(WhenOrderSubmitted());
            During(CartRequest.Pending, WhenCartReturned());
            During(MoneyReservationRequest.Pending, WhenMoneyReserved(), 
                WhenMoneyReservationRequestTimeoutExpired(),
                WhenMoneyReservationRequestFaulted());

            During(MoneyUnreservationRequest.Pending, WhenMoneyUnreserved());
            During(AwaitingConfirmation, WhenOrderConfirmed(), WhenOrderRejected());
            During(AwaitingDelivery, WhenOrderDelivered());
            During(AwaitingFeedback, WhenFeedbackReceived(), WhenFeedbackReceivingTimeoutExpired());
            During(AddFeedbackRequest.Pending, WhenFeedbackAdded());
            During(ArchiveOrderRequest.Pending, WhenOrderArchived());
            DuringAny(WhenOrderAborted());

            SetCompletedWhenFinalized();
        }

        private Task HandleUnhandledEvent(UnhandledEventContext<OrderState> context)
        {
            if (context.Event.Name.Contains("TimeoutExpired"))
            {
                _logger.LogDebug($"[{DateTime.Now}][SAGA] Ignored unhandled event: {context.Event.Name}");

                context.Ignore();
            }
            else
                context.Throw();

            return Task.CompletedTask;
        }

        private void BuildStateMachine()
        {
            Event(() => OrderSubmitted, x => x.CorrelateById(context => context.Message.OrderId));
            Event(() => OrderConfirmed, x => x.CorrelateById(context => context.Message.OrderId));
            Event(() => OrderRejected, x => x.CorrelateById(context => context.Message.OrderId));
            Event(() => OrderDelivered, x => x.CorrelateById(context => context.Message.OrderId));
            Event(() => ReceivedFeedback, x => x.CorrelateById(context => context.Message.OrderId));
            Event(() => OrderAborted, x => x.CorrelateById(context => context.Message.OrderId));

            Request(() => CartRequest, r =>
            {
                r.ServiceAddress = new Uri(_settings.CartServiceAddress);
                r.Timeout = TimeSpan.Zero;
            });

            Request(() => MoneyReservationRequest, r =>
            {
                r.ServiceAddress = new Uri(_settings.PaymentServiceAddress);
                r.Timeout = TimeSpan.Zero;
            });

            Request(() => MoneyUnreservationRequest, r =>
            {
                r.ServiceAddress = new Uri(_settings.PaymentServiceAddress);
                r.Timeout = TimeSpan.Zero;
            });

            Request(() => AddFeedbackRequest, r =>
            {
                r.ServiceAddress = new Uri(_settings.FeedbackServiceAddress);
                r.Timeout = TimeSpan.Zero;
            });

            Request(() => ArchiveOrderRequest, r =>
            {
                r.ServiceAddress = new Uri(_settings.HistoryServiceAddress);
                r.Timeout = TimeSpan.Zero;
            });

            Schedule(() => FeedbackReceivingTimeout, instance => instance.FeedbackReceivingTimeoutToken, 
                s =>
                {
                    s.Delay = TimeSpan.FromMinutes(1);
                    s.Received = r => r.CorrelateById(x => x.Message.OrderId);
                });
        }

        private EventActivities<OrderState> WhenOrderAborted()
        {
            return When(OrderAborted)
                .Then(context =>
                {
                    _logger.LogInformation($"[{DateTime.Now}][SAGA] Order aborted. CorrelationId: {context.Instance.CorrelationId}");

                    
                })
                .RespondAsync(x => x.Init<OrderAborted>(new
                {
                    OrderId = x.Instance.CorrelationId
                }))
                .Finalize();
        }

        private EventActivities<OrderState> WhenOrderSubmitted()
        {
            return When(OrderSubmitted)
                .Then(context =>
                {
                    _logger.LogInformation($"[{DateTime.Now}][SAGA] Order submitted. CorrelationId: {context.Instance.CorrelationId}");
                    context.Instance.SubmitDate = DateTimeOffset.Now;
                })
                .Request(CartRequest, x => x.Init<GetCart>(new { OrderId = x.Instance.CorrelationId }))
                .TransitionTo(CartRequest.Pending);
        }

        private EventActivities<OrderState> WhenCartReturned()
        {
            return When(CartRequest.Completed)
                .Then(x  =>
                {
                    x.Instance.Cart = FromDtoCartPositionToDbConverter.ConvertMany(x.Data.CartContent, x.Instance.CorrelationId);
                    x.Instance.TotalPrice = x.Data.TotalPrice;
                    _logger.LogInformation($"[{DateTime.Now}][SAGA] Cart returned. CorrelationId: {x.Instance.CorrelationId}");
                })
                .Request(MoneyReservationRequest, x => x.Init<ReserveMoney>(new
                {
                    OrderId = x.Data.OrderId,
                    Amount = x.Data.TotalPrice
                }))
                .TransitionTo(MoneyReservationRequest.Pending);
        }

        private EventActivities<OrderState> WhenMoneyReserved()
        {
            return When(MoneyReservationRequest.Completed)
                .Then(x =>
                {
                    _logger.LogInformation($"[{DateTime.Now}][SAGA] Money reserved. CorrelationId: {x.Instance.CorrelationId}");
                })
                .PublishAsync(x => x.Init<NewOrderConfirmationRequested>(new
                {
                    OrderId = x.Instance.CorrelationId,
                    Cart = FromDtoCartPositionToDbConverter.ConvertBackMany(x.Instance.Cart),
                    TotalPrice = x.Instance.TotalPrice
                }))
                .TransitionTo(AwaitingConfirmation);
        }

        private EventActivities<OrderState> WhenOrderConfirmed()
        {
            return When(OrderConfirmed)
                .Then(x =>
                {
                    _logger.LogInformation($"[{DateTime.Now}][SAGA] Order confirmed. CorrelationId: {x.Instance.CorrelationId}");

                    x.Instance.IsConfirmed = true;
                    x.Instance.ConfirmationDate = DateTimeOffset.Now;
                    x.Instance.Manager = x.Data.ConfirmManager;
                })
                .SendAsync(new Uri(_settings.DeliveryServiceAddress), x => x.Init<DeliveryOrder>(new
                {
                    OrderId = x.Instance.CorrelationId,
                    Cart = FromDtoCartPositionToDbConverter.ConvertBackMany(x.Instance.Cart)
                }))
                .TransitionTo(AwaitingDelivery);
        }

        private EventActivities<OrderState> WhenOrderRejected()
        {
            return When(OrderRejected)
                .Then(x =>
                {
                    _logger.LogInformation($"[{DateTime.Now}][SAGA] Order rejected. CorrelationId: {x.Instance.CorrelationId}");

                    x.Instance.Manager = x.Data.RejectManager;
                })
                .PublishAsync(x => x.Init<OrderRejected>(new
                {
                    OrderId = x.Instance.CorrelationId,
                    Reason = x.Data.Reason
                }))
                .Request(MoneyUnreservationRequest, x => x.Init<UnreserveMoney>(new
                {
                    OrderId = x.Instance.CorrelationId,
                    Amount = x.Instance.TotalPrice
                }))
                .TransitionTo(MoneyUnreservationRequest.Pending);
        }

        private EventActivities<OrderState> WhenOrderDelivered()
        {
            return When(OrderDelivered)
                .Then(x =>
                {
                    x.Instance.DeliveryDate = DateTimeOffset.Now;
                })
                .PublishAsync(x => x.Init<FeedbackRequested>(new
                {
                    OrderId = x.Instance.CorrelationId
                }))
                .Schedule(FeedbackReceivingTimeout, x => x.Init<FeedbackReceivingTimeoutExpired>(new
                {
                    OrderId = x.Instance.CorrelationId
                }))
                .TransitionTo(AwaitingFeedback);
        }

        private EventActivities<OrderState> WhenFeedbackReceived()
        {
            return When(ReceivedFeedback)
                .Unschedule(FeedbackReceivingTimeout)
                .Request(AddFeedbackRequest, x => x.Init<AddFeedback>(new
                {
                    OrderId = x.Instance.CorrelationId,
                    Text = x.Data.Text,
                    StarsAmount = x.Data.StarsAmount
                }))
                .TransitionTo(AddFeedbackRequest.Pending);
        }

        private EventActivities<OrderState> WhenFeedbackReceivingTimeoutExpired()
        {
            return When(FeedbackReceivingTimeout.Received)
                .Then(x => _logger.LogInformation(
                    $"[{DateTime.Now}][SAGA] Feedback receiving timed out. CorrelationId: {x.Instance.CorrelationId}"))
                .Request(ArchiveOrderRequest, x => x.Init<ArchiveOrder>(new
                {
                    OrderId = x.Instance.CorrelationId,
                    Cart = FromDtoCartPositionToDbConverter.ConvertBackMany(x.Instance.Cart),
                    TotalPrice = x.Instance.TotalPrice,
                    IsConfirmed = x.Instance.IsConfirmed,
                    SubmitDate = x.Instance.SubmitDate,
                    Manager = x.Instance.Manager,
                    ConfirmDate = x.Instance.ConfirmationDate,
                    DeliveredDate = x.Instance.DeliveryDate
                }))
                .TransitionTo(ArchiveOrderRequest.Pending);
        }

        private EventActivities<OrderState> WhenFeedbackAdded()
        {
            return When(AddFeedbackRequest.Completed)
                .Request(ArchiveOrderRequest, x => x.Init<ArchiveOrder>(new
                {
                    OrderId = x.Instance.CorrelationId,
                    Cart = FromDtoCartPositionToDbConverter.ConvertBackMany(x.Instance.Cart),
                    TotalPrice = x.Instance.TotalPrice,
                    IsConfirmed = x.Instance.IsConfirmed,
                    SubmitDate = x.Instance.SubmitDate,
                    Manager = x.Instance.Manager,
                    ConfirmDate = x.Instance.ConfirmationDate,
                    DeliveredDate = x.Instance.DeliveryDate
                }))
                .TransitionTo(ArchiveOrderRequest.Pending);
        }

        private EventActivities<OrderState> WhenOrderArchived()
        {
            return When(ArchiveOrderRequest.Completed)
                .Then(x => _logger.LogInformation($"[{DateTime.Now}][SAGA] Order archived. CorrelationId: {x.Instance.CorrelationId}"))
                .Finalize();
        }

        private EventActivities<OrderState> WhenMoneyUnreserved()
        {
            return When(MoneyUnreservationRequest.Completed)
                .Then(x => _logger.LogInformation(
                    $"[{DateTime.Now}][SAGA] Money unreserved. CorrelationId: {x.Instance.CorrelationId}"))
                .Request(ArchiveOrderRequest, x => x.Init<ArchiveOrder>(new
                {
                    OrderId = x.Instance.CorrelationId,
                    Cart = FromDtoCartPositionToDbConverter.ConvertBackMany(x.Instance.Cart),
                    TotalPrice = x.Instance.TotalPrice,
                    IsConfirmed = x.Instance.IsConfirmed,
                    SubmitDate = x.Instance.SubmitDate,
                    Manager = x.Instance.Manager,
                    ConfirmDate = x.Instance.ConfirmationDate,
                    DeliveredDate = x.Instance.DeliveryDate
                }))
                .TransitionTo(ArchiveOrderRequest.Pending);
        }

        private EventActivities<OrderState> WhenMoneyReservationRequestTimeoutExpired()
        {
            return When(MoneyReservationRequest.TimeoutExpired)
                .Then(x => _logger.LogWarning(
                    $"[{DateTime.Now}][SAGA] MoneyReservation request timed out. CorrelationId: {x.Instance.CorrelationId}"));
        }

        private EventActivities<OrderState> WhenMoneyReservationRequestFaulted()
        {
            return When(MoneyReservationRequest.Faulted)
                .Then(x => _logger.LogError(
                    $"[{DateTime.Now}][SAGA] MoneyReservation request failed. CorrelationId: {x.Instance.CorrelationId}"));
        }
    }
#nullable restore
}
