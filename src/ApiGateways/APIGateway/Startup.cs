﻿using Microsoft.AspNetCore.Builder;
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
using APIGateway.Infrastructure.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace APIGateway
{
    

    public class Startup
    {
        public Startup(IHostingEnvironment env, IConfiguration configuration)
        {
            Configuration = configuration;

            //var builder = new ConfigurationBuilder();
            //builder.SetBasePath(env.ContentRootPath)
            //       .AddJsonFile("appsettings.json", true, true)
            //       .AddJsonFile("configuration.json", optional: false, reloadOnChange: true)
            //       .AddEnvironmentVariables();

            //Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

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

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            })
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = Configuration["IdentityUrl"];
                    options.RequireHttpsMetadata = false;
                    options.ApiName = "apiGW";
                    options.SupportedTokens = SupportedTokens.Both;
                    options.ApiSecret = "secret";

                    options.EnableCaching = true;
                    options.CacheDuration = TimeSpan.FromMinutes(10);
                });

            // Add custom request aggregations
            services.AddOcelot(Configuration)
                .AddSingletonDefinedAggregator<Api1and2Aggregator>(); ;
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // Used to get correlationId from incoming requests or set new one if not existing
            app.UseMiddleware<ScopedSpecificSerilogLoggingMiddleware>();
            // Ocelot automaticly forwards headers to api request

            app.UseCors("CorsPolicy");
            app.UseAuthentication();
            app.UseMiddleware<UserSpecificSerilogLoggingMiddleware>();

            app.UseOcelot().Wait();
        }
    }
}
