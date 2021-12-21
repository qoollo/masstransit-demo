using System;
using System.Threading.Tasks;
using HistoryService.Contracts;
using MassTransit;

namespace OrderOrchestratorService.Tests.ConsumerStubs;

public class GetOrderFromArchiveConsumer : IConsumer<GetOrderFromArchive>
{
    public async Task Consume(ConsumeContext<GetOrderFromArchive> context)
    {
        await context.RespondAsync<GetOrderFromArchiveResponse>(new
        {

            OrderId = context.Message.OrderId,
            IsConfirmed = default(bool),
            SubmitDate = default(DateTimeOffset),
            Manager = default(string),
            ConfirmDate = default(DateTimeOffset?),
            DeliveredDate = default(DateTimeOffset?)
        });
    }
}