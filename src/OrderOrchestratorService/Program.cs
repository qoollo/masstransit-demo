using System;
using System.Reflection;
using CartService.Contracts;
using FeedbackService.Contracts;
using HistoryService.Contracts;
using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrderOrchestratorService.Configurations;
using OrderOrchestratorService.Consumers;
using OrderOrchestratorService.Database;
using OrderOrchestratorService.StateMachines.ArchivedOrderStateMachine;
using OrderOrchestratorService.StateMachines.OrderStateMachine;

namespace OrderOrchestratorService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = Assembly.GetExecutingAssembly().GetName().Name!;
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

                    var contextOptions = new DbContextOptionsBuilder()
                    .UseNpgsql(hostContext.Configuration.GetConnectionString("DefaultConnection"))
                    .Options;

                    using (var context = new StateMachinesDbContext(contextOptions))
                    {
                        context.Database.Migrate();
                    }   

                    var endpointsSection = hostContext.Configuration.GetSection("EndpointsConfiguration");
                    var endpointsConfig = endpointsSection.Get<EndpointsConfiguration>();

                    services.Configure<EndpointsConfiguration>(endpointsSection);

                    var rabbitMqSection = hostContext.Configuration.GetSection("RabbitMqConfiguration");
                    var rabbitMqConfig = rabbitMqSection.Get<RabbitMqConfiguration>();

                    services.AddDbContext<StateMachinesDbContext>(builder =>
                    {
                        builder.UseNpgsql(hostContext.Configuration.GetConnectionString("DefaultConnection"));
                    });

                    services.AddMassTransit(x =>
                    {
                        x.AddSagaRepository<OrderState>()
                            .EntityFrameworkRepository(r =>
                            {
                                r.ExistingDbContext<StateMachinesDbContext>();
                                r.ConcurrencyMode = ConcurrencyMode.Optimistic;
                            });

                        x.AddSagaStateMachine<OrderStateMachine, OrderState>(typeof(OrderStateMachineDefinition))
                            .Endpoint(e =>
                            {
                                e.Name = endpointsConfig.OrderStateMachineAddress;
                            });

                        x.AddSagaStateMachine<ArchivedOrderStateMachine, ArchivedOrderState>(typeof(ArchivedOrderStateMachineDefinition))
                            .InMemoryRepository()
                            .Endpoint(e =>
                            {
                                e.Name = endpointsConfig.ArchiveOrderStateMachineAddress;
                            });

                        x.AddDelayedMessageScheduler();
                        x.AddConsumer<GetAllOrdersStateConsumer>(typeof(GetAllOrdersStateConsumerDefinition));
                        x.AddConsumer<GetOrderStateConsumer>(typeof(GetOrderStateConsumerDefinition));
                        //x.AddConsumer<GetArchivedOrderConsumer>(typeof(GetArchivedOrderConsumerDefinition));

                        x.AddRequestClient<GetOrderFromArchive>(new Uri(endpointsConfig.HistoryServiceAddress!));
                        x.AddRequestClient<GetOrderFeedback>(new Uri(endpointsConfig.FeedbackServiceAddress!));
                        x.AddRequestClient<GetCart>(new Uri(endpointsConfig.CartServiceAddress!));


                        x.UsingRabbitMq((context, cfg) => 
                        {
                            cfg.UseBsonSerializer();

                            cfg.UseDelayedMessageScheduler();
                            
                            cfg.ConfigureEndpoints(context);

                            cfg.Host(rabbitMqConfig.Hostname, rabbitMqConfig.VirtualHost, h =>
                            {
                                h.Username(rabbitMqConfig.Username);
                                h.Password(rabbitMqConfig.Password);
                            });
                        });

                    }).AddMassTransitHostedService(true);
                });
    }
}
