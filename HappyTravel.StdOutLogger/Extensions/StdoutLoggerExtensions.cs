using System;
using HappyTravel.StdOutLogger.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace HappyTravel.StdOutLogger.Extensions
{
    public static class StdoutLoggerExtensions
    {
        public static IApplicationBuilder UseHttpContextLogging(
            this IApplicationBuilder builder, Action<HttpContextLoggingMiddlewareOptions> setupAction = default)
        {
            var options = new HttpContextLoggingMiddlewareOptions();
            setupAction?.Invoke(options);
            return builder.UseMiddleware<HttpContextLoggingMiddleware>(options);
        }


        public static ILoggerFactory AddStdOutLogger(this ILoggerFactory loggerFactory, IHttpContextAccessor httpContextAccessor, Action<StdOutLoggerOptions> setupAction = default)
        {
            var options = new StdOutLoggerOptions();
            setupAction?.Invoke(options);
            loggerFactory.AddProvider(new StdOutLoggerProvider(options, httpContextAccessor));
            return loggerFactory;
        }
    }
}