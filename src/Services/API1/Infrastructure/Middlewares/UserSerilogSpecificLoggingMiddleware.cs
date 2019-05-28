using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace API1.Infrastructure.Middlewares
{
    public class UserSerilogSpecificLoggingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<UserSerilogSpecificLoggingMiddleware> logger;

        public UserSerilogSpecificLoggingMiddleware(RequestDelegate next, ILogger<UserSerilogSpecificLoggingMiddleware> logger)
        {
            this.next = next ?? throw new ArgumentNullException(nameof(next));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Invoke(HttpContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var username = context.User.Identity.IsAuthenticated ? context.User.Identity.Name : "Unauthorized";

            try
            {
                //Add as many nested usings as needed, for adding more properties 
                using (LogContext.PushProperty("Username", username))
                {
                    await next.Invoke(context);
                }
            }
            //To make sure that we don't loose the scope in case of an unexpected error
            catch (Exception ex) when (LogOnUnexpectedError(ex))
            {
                return;
            }
        }


        private bool LogOnUnexpectedError(Exception ex)
        {
            logger.LogError(ex, "An unexpected exception occured!");
            return true;
        }
    }
}