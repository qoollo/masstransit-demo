using System.Threading.Tasks;
using ApiService.Contracts.UserApi;
using MassTransit;

namespace ApiService.Consumers
{
    public class FeedbackRequestedConsumer : IConsumer<FeedbackRequested>
    {
        public Task Consume(ConsumeContext<FeedbackRequested> context)
        {
            return Task.CompletedTask;
        }
    }
}
