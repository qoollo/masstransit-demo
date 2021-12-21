using System;
using System.Reflection;
using DeliveryService.Configurations;
using DeliveryService.Consumers;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace DeliveryService
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = Assembly.GetExecutingAssembly().GetName().Name!;
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    var endpointsConfigSection = hostContext.Configuration.GetSection("EndpointsConfiguration");
                    var endpointsConfig = endpointsConfigSection.Get<EndpointsConfiguration>();

                    var rabbitMqConfigSection = hostContext.Configuration.GetSection("RabbitMqConfiguration");
                    var rabbitMqConfig = rabbitMqConfigSection.Get<RabbitMqConfiguration>();

                    services.AddMassTransit(c =>
                    {
                        c.AddConsumer<DeliveryOrderConsumer>(typeof(DeliveryOrderConsumerDefinition))
                            .Endpoint(configurator =>
                            {
                                configurator.Name = endpointsConfig.DeliveryServiceAddress;
                            });

                        c.AddDelayedMessageScheduler();
                        c.UsingRabbitMq((context, configurator) =>
                        {
                            configurator.UseBsonSerializer();
                            configurator.Host(rabbitMqConfig.Hostname, rabbitMqConfig.VirtualHost, h =>
                            {
                                h.Username(rabbitMqConfig.Username);
                                h.Password(rabbitMqConfig.Password);
                            });


                            configurator.UseDelayedMessageScheduler();
                            configurator.ConfigureEndpoints(context);
                        });
                    });

                    services.AddMassTransitHostedService(true);
                });
    }
}
