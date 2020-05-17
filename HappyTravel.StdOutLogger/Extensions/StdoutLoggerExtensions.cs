using System;
using HappyTravel.StdOutLogger.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace HappyTravel.StdOutLogger.Extensions
{
    public static class StdOutLoggerExtensions
    {
        public static ILoggingBuilder AddStdOutLogger(this ILoggingBuilder builder, Action<StdOutLoggerOptions> setupAction)
        {
            builder.Services.AddHttpContextAccessor();
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, StdOutLoggerProvider>());
            builder.Services.Configure(setupAction);
            return builder;
        }
        
        
        public static IApplicationBuilder UseHttpContextLogging(
            this IApplicationBuilder builder, Action<HttpContextLoggingMiddlewareOptions>? setupAction = default)
        {
            var options = new HttpContextLoggingMiddlewareOptions();
            setupAction?.Invoke(options);
            return builder.UseMiddleware<HttpContextLoggingMiddleware>(options);
        }
    }
}