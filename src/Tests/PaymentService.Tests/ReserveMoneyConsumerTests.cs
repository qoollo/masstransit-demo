using System;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Xunit;
using MassTransit.Testing;
using Microsoft.Extensions.Logging;
using Moq;
using PaymentService.Consumers;
using PaymentService.Contracts;

namespace PaymentService.Tests
{
    public class ReserveMoneyConsumerTests
    {
        [Fact]
        public async Task MoneyReservationTest()
        {
            var harness = new InMemoryTestHarness()
            {
                TestTimeout = TimeSpan.FromSeconds(2)
            };

            var consumerHarness = harness.Consumer(() =>
            {
                return new ReserveMoneyConsumer(new Mock<ILogger<ReserveMoneyConsumer>>().Object);
            });

            await harness.Start();

            try
            {
                var orderId = Guid.NewGuid();
                var amount = 100;

                var client = await harness.ConnectRequestClient<ReserveMoney>();

                var response = await client.GetResponse<MoneyReserved>(new
                {

                    OrderId = orderId,
                    Amount = amount
                });

                Assert.Equal(orderId, response.Message.OrderId);

                Assert.True(consumerHarness.Consumed.Select<ReserveMoney>().Any());
                Assert.True(harness.Sent.Select<MoneyReserved>().Any());

            }
            finally
            {
                await harness.Stop();
            }
        }


        [Theory]
        [InlineData(0)]
        [InlineData(-20)]
        [InlineData(-1000)]
        public async Task ShouldNotReserveMoneyWhenAmointIsLessThanZero(int amount)
        {
            var harness = new InMemoryTestHarness()
            {
                TestTimeout = TimeSpan.FromSeconds(2)
            };

            var consumerHarness = harness.Consumer(() =>
            {
                return new ReserveMoneyConsumer(new Mock<ILogger<ReserveMoneyConsumer>>().Object);
            });

            await harness.Start();

            try
            {
                var orderId = Guid.NewGuid();

                var client = await harness.ConnectRequestClient<ReserveMoney>();

                var response = await client.GetResponse<ErrorReservingMoney>(new
                {

                    OrderId = orderId,
                    Amount = amount
                });

                Assert.Equal(orderId, response.Message.OrderId);

                Assert.True(consumerHarness.Consumed.Select<ReserveMoney>().Any());
                
                Assert.True(harness.Sent.Select<ErrorReservingMoney>().Any());
                Assert.False(harness.Sent.Select<MoneyReserved>().Any());

            }
            finally
            {
                await harness.Stop();
            }
        }
    }
}