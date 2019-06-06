using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens;
using IdentityModel.AspNetCore;
using MVCClient.Infrastructure;
using MVCClient.Services;
using Polly;
using Polly.Extensions.Http;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using MVCClient.Infrastructure.Middlewares;

namespace MVCClient
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
            services
                .AddCustomMvc(Configuration)
                .AddHttpClientServices(Configuration)
                .AddCustomAuthentication(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseMiddleware<ScopedSpecificSerilogLoggingMiddleware>();
            // For forwarding headers in requests
            // https://github.com/alefranz/HeaderPropagation
            app.UseHeaderPropagation();

            app.UseAuthentication();
            app.UseMiddleware<UserSpecificSerilogLoggingMiddleware>();

            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }

    static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomMvc(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.Configure<AppSettings>(configuration);

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            return services;
        }

        // Adds all Http client services (like Service-Agents) using resilient Http requests based on HttpClient factory and Polly's policies 
        public static IServiceCollection AddHttpClientServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //register delegating handlers
            services.AddTransient<HttpClientAuthorizationDelegatingHandler>();

            // For forwarding headers in requests
            // https://github.com/alefranz/HeaderPropagation
            services.AddHeaderPropagation(o => o.Headers.Add("CorrelationID"));

            //set 5 min as the lifetime for each HttpMessageHandler int the pool
            services.AddHttpClient("extendedhandlerlifetime")
                .SetHandlerLifetime(TimeSpan.FromMinutes(5));

            //add http client services
            services.AddHttpClient<IApi1Service, Api1Service>()
                   .SetHandlerLifetime(TimeSpan.FromMinutes(5))  //Sample. Default lifetime is 2 minutes
                   .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
                   .AddHeaderPropagation(o => o.Headers.Add("CorrelationID"))
                   .AddPolicyHandler(GetRetryPolicy())
                   .AddPolicyHandler(GetCircuitBreakerPolicy());
            

            return services;
        }

        public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); //custom

            var identityUrl = configuration.GetValue<string>("IdentityUrl");
            var callBackUrl = configuration.GetValue<string>("CallBackUrl");

            services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddCookie(setup => setup.ExpireTimeSpan = TimeSpan.FromHours(3))
                // AddAutomaticTokenManagement is used to automaticlly renew access_token with renew_token
                .AddAutomaticTokenManagement(opt => 
                {
                    opt.RefreshBeforeExpiration = TimeSpan.FromMinutes(20);
                })
                .AddOpenIdConnect(options =>
                {
                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

                    options.Authority = identityUrl.ToString();
                    options.SignedOutRedirectUri = callBackUrl.ToString();
                    options.RequireHttpsMetadata = false; // PRODUCTION - should stay true

                    options.ClientId = "mvc";
                    options.ClientSecret = "secret";
                    options.ResponseType = "code id_token";

                    options.SaveTokens = true;
                    options.GetClaimsFromUserInfoEndpoint = true;

                    options.Scope.Add("api1");
                    options.Scope.Add("api2");
                    options.Scope.Add("apiGW");
                    options.Scope.Add("offline_access");
                    options.Scope.Add("profile");
                    options.Scope.Add("openid");
                    //options.Scope.Add("roles"); // role as resource
                    options.Scope.Add("signalrhub");

                    options.ClaimActions.MapJsonKey("role", "role", "role"); // mapping role claims
                    options.ClaimActions.MapJsonKey("website", "website");

                    options.TokenValidationParameters = new TokenValidationParameters // role as resource
                    {
                        NameClaimType = "name",
                        RoleClaimType = "role"
                    };
                });

            return services;
        }

        static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
              .HandleTransientHttpError()
              .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
              .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        }

        static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
        }

        /// <summary>
        /// GetNewCorrelationID is used to make new Guid-CorrelationID and add it to LogContexts properties
        /// </summary>
        /// <returns> Guid-CorrelationID</returns>
        static string GetNewCorrelationID() // TODO: not needed?
        {
            string correlationId = Guid.NewGuid().ToString();
            try
            {
                //Add as many nested usings as needed, for adding more properties 
                LogContext.PushProperty("CorrelationID", correlationId, true);
                return correlationId;
            }
            //To make sure that we don't loose the scope in case of an unexpected error
            catch (Exception ex)
            {
                return "couldnt generate correlationId. Error: " + ex.Message;
            }
        }
    }
}
