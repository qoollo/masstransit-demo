using CartService.Consumers;
using CartService.Database;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using CartService.Configurations;
using CartService.Database.Repositories.Interfaces;
using CartService.Database.Repositories;
using System.Reflection;

namespace CartService
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

                    services.AddTransient<ICartRepository, CartRepository>();
                    services.AddTransient<ICartPositionRepository, CartPositionRepository>();
                    services.AddTransient<IGoodRepository, GoodRepository>();

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
                        x.AddConsumer<AddCartPositionConsumer>(typeof(AddCartPositionConsumerDefinition))
                            .Endpoint(cfg =>
                            {
                                cfg.Name = endpointsConfig.CartServiceAddress;
                            });

                        x.AddConsumer<RemoveCartPositionConsumer>(typeof(RemoveCartPositionConsumerDefinition))
                            .Endpoint(cfg =>
                            {
                                cfg.Name = endpointsConfig.CartServiceAddress;
                            });

                        x.AddConsumer<GetCartConsumer>(typeof(GetCartConsumerDefinition))
                            .Endpoint(cfg =>
                            {
                                cfg.Name = endpointsConfig.CartServiceAddress;
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
