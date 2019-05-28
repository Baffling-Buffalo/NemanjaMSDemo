using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using API2.Infrastructure.Middlewares;
using Microsoft.AspNetCore.Http;
using Serilog.Context;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using API2.Models;
using BuildingBlocksEventBusProjects.EventBusRabbitMQ;
using RabbitMQ.Client;
using BuildingBlocks.EventBusProjects.EventBus.Abstractions;
using BuildingBlocks.EventBusProjects.EventBus;
using API2.IntegrationEvents.Handlers;
using API2.IntegrationEvents.Events;
using Autofac;
using Autofac.Extensions.DependencyInjection;

namespace API2
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        //This method gets called by the runtime.Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddCustomMVC(Configuration)
                .AddCustomDbContext(Configuration);

            //Used to know where is IdentityServer4 located to go validate the token
            //and set the name of this API as audience, which is used at identityserver
            //as scope to which client can  have access to
            services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = Configuration["IdentityServer"];
                    options.RequireHttpsMetadata = false;
                    options.ApiName = "api2";

                    options.EnableCaching = true;
                    options.CacheDuration = TimeSpan.FromMinutes(10);
                });

            services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

                var factory = new ConnectionFactory()
                {
                    HostName = "localhost",
                    DispatchConsumersAsync = true,
                    UserName = "guest",
                    Password = "guest"
                };

                var retryCount = 5;

                return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
            });

            services.RegisterEventBus(Configuration);

            //configure autofac
            var container = new ContainerBuilder();
            container.Populate(services);

            return new AutofacServiceProvider(container.Build());
        }

        //This method gets called by the runtime.Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // app.UseHttpsRedirection();  // Not needed if using gateway, which will handle https
                // app.UseHsts(); // Not needed if using gateway, which will handle https
            }
            // Used to get correlationId from incoming requests or set new one if not existing
            app.UseMiddleware<ScopedSerilogSpecificLoggingMiddleware>();
            // app.UseCors("default");
            app.UseAuthentication(); // Not needed if gateway handles authentication, authorization and scopes
            app.UseMiddleware<UserSerilogSpecificLoggingMiddleware>();

            app.UseMvc();

            ConfigureEventBus(app);
        }

        private void ConfigureEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
            eventBus.Subscribe<DataUpdatedIntegrationEvent, DataUpdatedIntegrationEventHandler>();
        }
    }

    static class ServisCollectionExtension
    {
        public static IServiceCollection AddCustomMVC(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            //.AddControllersAsServices();

            return services;
        }

        public static IServiceCollection AddCustomDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<Api2Context>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("Api2Db"),
                                     sqlServerOptionsAction: sqlOptions =>
                                     {
                                         sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                                         //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                                         sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                                     });
            });

            return services;
        }

        public static IServiceCollection RegisterEventBus(this IServiceCollection services, IConfiguration configuration)
        {
            //var subscriptionClientName = configuration["SubscriptionClientName"];
            var subscriptionClientName = "Api2";

            services.AddSingleton<IEventBus, EventBusRabbitMQ>(sp =>
            {
                var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ>>();
                var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                var retryCount = 5;

                return new EventBusRabbitMQ(rabbitMQPersistentConnection, logger, iLifetimeScope, eventBusSubcriptionsManager, subscriptionClientName, retryCount);
            });

            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
            services.AddTransient<DataUpdatedIntegrationEventHandler>();

            return services;
        }
    }
}
