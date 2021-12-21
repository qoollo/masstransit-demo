using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiService.Contracts.ManagerApi;
using ApiService.Contracts.UserApi;
using Automatonymous.Graphing;
using Automatonymous.Visualizer;
using CartService.Contracts;
using DeliveryService.Contracts;
using MassTransit;
using MassTransit.Saga.InMemoryRepository;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using OrderOrchestratorService.Database.Models;
using OrderOrchestratorService.StateMachines.OrderStateMachine;
using OrderOrchestratorService.Tests.ConsumerStubs;
using PaymentService.Contracts;
using Xunit;
using Xunit.Abstractions;
using OrderMachine = OrderOrchestratorService.StateMachines.OrderStateMachine.OrderStateMachine;

namespace OrderOrchestratorService.Tests.StateMachines;

public class OrderStateMachineTests :
    IClassFixture<StateMachineTestFixture<OrderMachine, OrderState>>
{
    private readonly StateMachineTestFixture<OrderMachine, OrderState> _fixture;
    private readonly ITestOutputHelper _output;

    public OrderStateMachineTests(StateMachineTestFixture<OrderMachine, OrderState> fixture,
        ITestOutputHelper output)
    {
        _fixture = fixture;
        _output = output;
    }


    [Fact]
    public async Task SagaShouldConsumeSubmitOrderMessage()
    {
        var orderId = NewId.NextGuid();
        var username = "TestUser";

        await _fixture.TestHarness.Bus.Publish<OrderSubmitted>(new
        {

            OrderId = orderId,
            UserName = username
        });

        // Assert that endpoint consumed message
        Assert.True(_fixture.TestHarness.Consumed.Select<OrderSubmitted>().Any());

        // Assert that saga consumed message
        Assert.True(_fixture.SagaHarness.Consumed.Select<OrderSubmitted>().Any());

        // Assert that saga created
        Assert.True(await _fixture.SagaHarness.Created.Any(s => s.CorrelationId == orderId),
            "Saga was not created");
    }

    [Fact]
    public async Task InstanceShouldTransitToGetCartStateWhenSumbitOrderMessageConsumed()
    {

        var orderId = NewId.NextGuid();
        var username = "TestUser";

        await _fixture.TestHarness.Bus.Publish<OrderSubmitted>(new
        {

            OrderId = orderId,
            UserName = username
        });

        Assert.NotNull(await _fixture.SagaHarness.Exists(orderId, _fixture.Machine.CartRequest.Pending));
    }

    [Fact]
    public async Task ShouldGetCartMessageWhenSumbitOrderMessageConsumed()
    {
        var orderId = NewId.NextGuid();
        var username = "TestUser";

        await _fixture.TestHarness.Bus.Publish<OrderSubmitted>(new
        {

            OrderId = orderId,
            UserName = username
        });

        Assert.True(_fixture.TestHarness.Sent
            .Select<GetCart>()
            .Any());
    }

    /*
     * Below you can see an example of adding some stuff to service provider.
     * This things depend on TStateMachine dependencies.
     * I added a consumer stub as saga is making a request and waiting for response.
     * 
     * Note: Adding services demands reinitialization of service provider.
     *       But this operation is really fast. So you should not care about it.
     */
    [Fact]
    public async Task ShouldMakeGetCartRequestWhenOrderSubmittedMessageConsumed()
    {
        await AddRequiredServices();

        var orderId = NewId.NextGuid();

        await _fixture.TestHarness.Bus.Publish<OrderSubmitted>(new
        {
            OrderId = orderId
        });

        var consumerHarness = _fixture.ServiceProvider
            .GetRequiredService<IConsumerTestHarness<GetCartConsumer>>();

        Assert.True(consumerHarness.Consumed.Select<GetCart>().Any(),
            "Get cart consumer did not consume message");

        Assert.True(_fixture.SagaHarness.Consumed.Select<GetCartResponse>().Any());
    }

    private async Task AddRequiredServices()
    {
        _fixture.ConfigureMassTransit = (cfg) =>
        {
            /*
             * If your state machine always requires service address to be specified
             * you must add Endpoint to the consumer.
             * The endpoint value should be the same as in your mocked options
             */
            cfg.AddConsumer<GetCartConsumer>()
                .Endpoint(c => c.Name = "cart-service");
            cfg.AddConsumerTestHarness<GetCartConsumer>();

            cfg.AddConsumer<ReserveMoneyConsumer>()
                .Endpoint(c => c.Name = "payment-service");
            cfg.AddConsumerTestHarness<ReserveMoneyConsumer>();
        };

        await _fixture.DisposeAsync();
        await _fixture.InitializeAsync();
    }

    [Fact]
    public async Task ShouldMakeReserveMoneyRequestWhenGetCartResponseConsumed()
    {
        await AddRequiredServices();

        var orderId = NewId.NextGuid();

        await _fixture.TestHarness.Bus.Publish<OrderSubmitted>(new
        {
            OrderId = orderId
        });

        var getCartConsumerHarness = _fixture.ServiceProvider
            .GetRequiredService<IConsumerTestHarness<GetCartConsumer>>();

        Assert.True(_fixture.SagaHarness.Consumed.Select<GetCartResponse>().Any(),
            "Saga did not consume GetCartResponse message");

        var consumerHarness = _fixture.ServiceProvider
            .GetRequiredService<IConsumerTestHarness<ReserveMoneyConsumer>>();

        Assert.True(consumerHarness.Consumed.Select<ReserveMoney>().Any());
    }

    /*
     * Below I show how to add saga to storage
     */
    [Fact]
    public async Task ShouldProduceDeliverOrderMessageWhenConfirmOrderConsumed()
    {
        var orderId = NewId.NextGuid();
        
        var instance = new SagaInstance<OrderState>(new OrderState
        {
            CorrelationId = orderId,
            CurrentState = _fixture.ConvertStateToInt(x => x.AwaitingConfirmation),
            // You need to add some car pos or converter in state machine will raise an exception
            Cart = new List<CartPosition>() { new CartPosition(NewId.NextGuid(), orderId, "Cart pos name", 5, 100) }
        });

        _fixture.Repository.Add(instance);

        await _fixture.TestHarness.Bus.Publish<ConfirmOrder>(new
        {
            OrderId = orderId,
            ConfirmManager = "Manager name"
        });

        Assert.True(_fixture.SagaHarness.Consumed.Select<ConfirmOrder>().Any(), "Saga did not consume ConfirmOrder message");


        // Assert that saga sent DeliverOrder message
        Assert.True(_fixture.TestHarness.Sent.Select<DeliveryOrder>().Any());

        // Assert that saga transited to next state
        Assert.NotNull(await _fixture.SagaHarness.Exists(orderId, _fixture.Machine.AwaitingDelivery));
    }

    /*
    * Below I show how to work with system time
    */
    [Fact]
    public async Task ShouldMakeArchiveRequestWhenAwaitingFeedbackScheduleExpires()
    {
        var orderId = NewId.NextGuid();

        var instance = new SagaInstance<OrderState>(new OrderState
        {
            CorrelationId = orderId,
            CurrentState = _fixture.ConvertStateToInt(_fixture.Machine.AwaitingDelivery),
            // You need to add some car pos or converter in state machine will raise an exception
            Cart = new List<CartPosition>() { new CartPosition(NewId.NextGuid(), orderId, "Cart pos name", 5, 100) }
        });

        _fixture.Repository.Add(instance);

        await _fixture.TestHarness.Bus.Publish<OrderDelivered>(new
        {
            OrderId = orderId
        });

        Assert.NotNull(await _fixture.SagaHarness.Exists(orderId, _fixture.Machine.AwaitingFeedback));

        await _fixture.AdvanceSystemTime(TimeSpan.FromMinutes(1));

        // Saga transited to next step
        Assert.NotNull(await _fixture.SagaHarness.Exists(orderId, _fixture.Machine.ArchiveOrderRequest.Pending));
    }

    [Fact]
    public void GetDigraph()
    {
        var graph = _fixture.Machine.GetGraph();

        var generator = new StateMachineGraphvizGenerator(graph);

        _output.WriteLine(generator.CreateDotFile());
    }
}