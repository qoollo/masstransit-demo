using System;
using System.Reflection;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using PaymentService.Configurations;
using PaymentService.Consumers;

namespace PaymentService
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
                        c.AddConsumer<ReserveMoneyConsumer>(typeof(ReserveMoneyConsumerDefinition))
                            .Endpoint(configurator =>
                            {
                                configurator.Name = endpointsConfig.PaymentServiceAddress;
                            });
                        
                        c.AddConsumer<UnreserveMoneyConsumer>()
                            .Endpoint(configurator =>
                            {
                                configurator.Name = endpointsConfig.PaymentServiceAddress;
                            });
                        
                        c.UsingRabbitMq((context, configurator) =>
                        {
                            configurator.UseBsonSerializer();
                            configurator.Host(rabbitMqConfig.Hostname, rabbitMqConfig.VirtualHost, h =>
                            {
                                h.Username(rabbitMqConfig.Username);
                                h.Password(rabbitMqConfig.Password);
                            });

                            configurator.ConfigureEndpoints(context);
                        });
                    });
                    services.AddMassTransitHostedService(true);
                });
    }
}