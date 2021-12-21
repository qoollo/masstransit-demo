using System;
using System.Threading.Tasks;
using ApiService.Contracts.ManagerApi;
using CartService.Contracts;
using FeedbackService.Contracts;
using HistoryService.Contracts;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using MassTransit.Testing;
using OrderOrchestratorService.Consumers;
using OrderOrchestratorService.Tests.ConsumerStubs;

namespace OrderOrchestratorService.Tests
{
    public class GetArchivedOrderConsumerTests
    {
        [Fact]
        public async Task Test()
        {
            var serviceCollection = new ServiceCollection()
                .AddLogging()
                .AddMassTransitInMemoryTestHarness(cfg =>
                {
                    cfg.AddRequestClient<GetCart>();
                    cfg.AddRequestClient<GetOrderFromArchive>();
                    cfg.AddRequestClient<GetOrderFeedback>();

                    cfg.AddConsumer<GetCartConsumer>();
                    cfg.AddConsumer<GetOrderFeedbackConsumer>();
                    cfg.AddConsumer<GetOrderFromArchiveConsumer>();

                    cfg.AddConsumer<GetArchivedOrderConsumer>();
                    cfg.AddConsumerTestHarness<GetArchivedOrderConsumer>();
                });

            var provider = serviceCollection.BuildServiceProvider(true);

            var harness = provider.GetRequiredService<InMemoryTestHarness>();

            await harness.Start();

            try
            {
                var orderId = Guid.NewGuid();

                var bus = provider.GetRequiredService<IBus>();
                var client = bus.CreateRequestClient<GetArchivedOrder>();

                var response = await client.GetResponse<GetArchivedOrderResponse>(new
                {
                    OrderId = orderId
                });

                var consumerTestHarness = provider
                    .GetRequiredService<IConsumerTestHarness<GetArchivedOrderConsumer>>();

                Assert.True(await harness.Consumed.Any<GetArchivedOrder>());
                Assert.True(await consumerTestHarness.Consumed.Any<GetArchivedOrder>());
                
                Assert.True(await harness.Sent.Any<GetArchivedOrderResponse>());

                Assert.NotNull(response);
                Assert.Equal(orderId, response.Message.OrderId);
            }
            finally
            {
                await provider.DisposeAsync();
                await harness.Stop();
            }
        }
    }
}