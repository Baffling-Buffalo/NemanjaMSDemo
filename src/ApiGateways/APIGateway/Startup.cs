using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using IdentityServer4.AccessTokenValidation;
using IdentityModel;
using APIGateway.Aggregator;
using APIGateway.BuildingBlocks.Middlewares;

namespace APIGateway
{
    

    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(env.ContentRootPath)
                   .AddJsonFile("appsettings.json", true, true)
                   //add configuration.json
                   .AddJsonFile("configuration.json", optional: false, reloadOnChange: true)
                   .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy",
                    builder => builder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .SetIsOriginAllowed((host) => true)
                    .AllowCredentials());
            });

            services.AddHeaderPropagation(o => o.Headers.Add("CorrelationID"));

            services.AddAuthentication()
                .AddIdentityServerAuthentication("Bearer", options =>
                {
                    options.Authority = Configuration["IdentityServer"];
                    options.RequireHttpsMetadata = false;
                    options.ApiName = "apiGW";
                    options.SupportedTokens = SupportedTokens.Both;
                    //options.ApiSecret = "secret"; );

                    options.EnableCaching = true;
                    options.CacheDuration = TimeSpan.FromMinutes(10);
                });


            // Add custom request aggregations
            services.AddOcelot(Configuration)
                .AddSingletonDefinedAggregator<Api1and2Aggregator>(); ;
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMiddleware<ScopedSerilogSpecificLoggingMiddleware>();
            app.UseHeaderPropagation();

            app.UseCors("CorsPolicy");
            app.UseAuthentication();
            app.UseOcelot().Wait();
        }
    }
}
