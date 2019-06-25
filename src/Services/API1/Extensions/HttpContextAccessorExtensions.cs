using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API1.Extensions
{
    public static class HttpContextAccessorExtensions
    {
        public static string GetCorrelationId(this IHttpContextAccessor context)
        {
            var correlationHeader = context.HttpContext.Request.Headers["CorrelationId"];

            if (!string.IsNullOrWhiteSpace(correlationHeader))
                return correlationHeader;

            else return "";
        }

        public static string GetCorrelationId(this HttpContext context)
        {
            var correlationHeader = context.Request.Headers["CorrelationId"];

            if (!string.IsNullOrWhiteSpace(correlationHeader))
                return correlationHeader;

            else return "";
        }
    }
}
