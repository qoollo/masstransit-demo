using FeedbackService.Configurations;
using FeedbackService.Consumers;
using FeedbackService.Database;
using FeedbackService.Database.Repositories;
using FeedbackService.Database.Repositories.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace FeedbackService
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
                    var contextOptions = new DbContextOptionsBuilder()
                    .UseNpgsql(hostContext.Configuration.GetConnectionString("DefaultConnection"))
                    .Options;

                    using (var context = new NpgSqlContext(contextOptions))
                    {
                        context.Database.Migrate();
                    }

                    services.AddTransient<IFeedbackRepository, FeedbackRepository>();

                    services.AddDbContext<NpgSqlContext>(opt =>
                        opt.UseNpgsql(hostContext.Configuration.GetConnectionString("DefaultConnection")),
                    ServiceLifetime.Transient,
                    ServiceLifetime.Transient);

                    var endpointsSection = hostContext.Configuration.GetSection("EndpointsConfiguration");
                    var endpointsConfig = endpointsSection.Get<EndpointsConfiguration>();

                    var rabbitMqSection = hostContext.Configuration.GetSection("RabbitMqConfiguration");
                    var rabbitMqConfig = rabbitMqSection.Get<RabbitMqConfiguration>();

                    services.AddMassTransit(x =>
                    {
                        x.AddConsumer<AddFeedbackConsumer>(typeof(AddFeedbackConsumerDefinition))
                            .Endpoint(cfg =>
                            {
                                cfg.Name = endpointsConfig.FeedbackServiceAddress;
                            });

                        x.AddConsumer<GetOrderFeedbackConsumer>(typeof(GetOrderFeedbackConsumerDefinition))
                            .Endpoint(cfg =>
                            {
                                cfg.Name = endpointsConfig.FeedbackServiceAddress;
                            });

                        x.UsingRabbitMq((context, cfg) =>
                        {
                            cfg.UseBsonSerializer();
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
