using System;
using HappyTravel.StdOutLogger.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace HappyTravel.StdOutLogger.Extensions
{
    public static class StdoutLoggerExtensions
    {
        public static ILoggingBuilder AddStdOut(this ILoggingBuilder builder)
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, StdOutLoggerProvider>());
            builder.Services.TryAddScoped<IHttpContextLogger, HttpContextLogger>();
            return builder;
        }


        public static ILoggingBuilder AddStdOut(this ILoggingBuilder builder, Action<StdOutLoggerOptions> configure)
        {
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            builder.AddStdOut();
            builder.Services.Configure(configure);

            return builder;
        }


        public static IApplicationBuilder UseHttpContextLogging(
            this IApplicationBuilder builder, Action<HttpContextLoggingMiddlewareOptions> setupAction = default)
        {
            var options = new HttpContextLoggingMiddlewareOptions();
            setupAction?.Invoke(options);
            return builder.UseMiddleware<HttpContextLoggingMiddleware>(options);
        }
    }
}