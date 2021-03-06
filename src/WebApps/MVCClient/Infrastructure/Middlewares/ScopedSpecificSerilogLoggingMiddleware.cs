using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace MVCClient.Infrastructure.Middlewares
{
    public class ScopedSpecificSerilogLoggingMiddleware
    {
        const string CORRELATION_ID_HEADER_NAME = "CorrelationID";
        private readonly RequestDelegate next;
        private readonly ILogger<ScopedSpecificSerilogLoggingMiddleware> logger;
        private readonly IHttpContextAccessor httpAccessor;

        public ScopedSpecificSerilogLoggingMiddleware(RequestDelegate next, ILogger<ScopedSpecificSerilogLoggingMiddleware> logger, IHttpContextAccessor httpAccessor)
        {
            this.next = next ?? throw new ArgumentNullException(nameof(next));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.httpAccessor = httpAccessor ?? throw new ArgumentNullException(nameof(httpAccessor));
        }

        public async Task Invoke(HttpContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var correlationId = GetOrAddCorrelationHeader(context);

            try
            {
                //Add as many nested usings as needed, for adding more properties 
                using(LogContext.PushProperty(CORRELATION_ID_HEADER_NAME, correlationId, true))
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

        private string GetOrAddCorrelationHeader(HttpContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            if(string.IsNullOrWhiteSpace(context.Request.Headers[CORRELATION_ID_HEADER_NAME]))
                context.Request.Headers[CORRELATION_ID_HEADER_NAME] = Guid.NewGuid().ToString();

            return context.Request.Headers[CORRELATION_ID_HEADER_NAME];
        }

        private bool LogOnUnexpectedError(Exception ex)
        {
            logger.LogError(ex, "An unexpected exception occured!");
            return true;
        }
    }
}