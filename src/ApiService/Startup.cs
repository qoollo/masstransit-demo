using ApiService.Configurations;
using ApiService.Consumers;
using ApiService.Contracts.ManagerApi;
using ApiService.Contracts.MonitoringApi;
using ApiService.Contracts.UserApi;
using ApiService.Models.Implementations;
using ApiService.Models.Interfaces;
using MassTransit;
using MassTransit.PrometheusIntegration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Prometheus;

namespace ApiService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "UserWebApi", Version = "v1" });
            });

            var routingSection = Configuration.GetSection("RoutingConfiguration");
            var routingConfig = routingSection.Get<RoutingConfiguration>();

            var rabbitMqSection = Configuration.GetSection("RabbitMqConfiguration");
            var rabbitMqConfig = rabbitMqSection.Get<RabbitMqConfiguration>();

            services.AddTransient<IRoutingConfiguration>(services => routingConfig);

            services.AddMassTransit(cfg =>
            {
                cfg.AddConsumer<NewOrderConfirmationRequestedConsumer>(typeof(NewOrderConfirmationRequestedConsumerDefinition))
                .Endpoint(e =>
                {
                    e.Name = routingConfig.ApiServiceAddress;
                });

                cfg.AddConsumer<OrderRejectedConsumer>(typeof(OrderRejectedConsumerDefinition))
                    .Endpoint(e =>
                    {
                        e.Name = routingConfig.ApiServiceAddress;
                    });

                cfg.AddConsumer<FeedbackRequestedConsumer>(typeof(FeedbackRequestedConsumerDefinition))
                    .Endpoint(e =>
                    {
                        e.Name = routingConfig.ApiServiceAddress;
                    });

                cfg.AddConsumer<GetArchivedOrderResponseConsumer>(typeof(GetArchivedOrderResponseConsumerDefinition))
                    .Endpoint(e =>
                    {
                        e.Name = routingConfig.ApiServiceAddress;
                    });

                cfg.AddRequestClient<GetAllOrdersState>();
                cfg.AddRequestClient<GetOrderState>();
                cfg.AddRequestClient<GetArchivedOrder>();
                cfg.AddRequestClient<AbortOrder>();

                cfg.UsingRabbitMq((context, config) =>
                {
                    config.UseBsonSerializer();
                    config.ConfigureEndpoints(context);

                    config.Host(rabbitMqConfig.Hostname, rabbitMqConfig.VirtualHost, h =>
                    {
                        h.Username(rabbitMqConfig.Username);
                        h.Password(rabbitMqConfig.Password);
                    });

                    config.UsePrometheusMetrics(serviceName: "api_service");
                });
            })
            .AddMassTransitHostedService(true);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "UserWebApi v1"));

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapMetrics();
                endpoints.MapControllers();
            });
        }
    }
}
