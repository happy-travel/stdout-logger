using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using HappyTravel.StdOutLogger.Extensions;
using HappyTravel.StdOutLogger.Internals;
using HappyTravel.StdOutLogger.Models;
using HappyTravel.StdOutLogger.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace HappyTravel.StdOutLogger
{
    public class HttpContextLoggingMiddleware
    {
        public HttpContextLoggingMiddleware(RequestDelegate next, HttpContextLoggingMiddlewareOptions options)
        {
            _options = options;
            _next = next;
        }


        public async Task Invoke(HttpContext httpContext,
            ILogger<HttpContextLoggingMiddleware> logger)
        {
            if (_options.IgnoredPaths.Contains(httpContext.Request.Path.Value.ToUpperInvariant()))
            {
                await _next(httpContext);
                return;
            }

            var formattedHttpRequest = await HttpLogHelper.GetFormattedHttpRequest(httpContext.Request);
            var createdAt = DateTime.UtcNow;
            
            await _next(httpContext);

            var formattedHttpResponse = HttpLogHelper.GetFormattedHttpResponse(httpContext.Response);
            
            logger.Log(LogLevel.Information,
                _eventId,
                new HttpContextLogEntry(
                    createdAt,
                    formattedHttpRequest,
                    formattedHttpResponse),
                null,
                (entry, exception) => string.Empty);
            
        }

        
        private readonly EventId _eventId = new EventId(MiddlewareEvenId, "HttpLoggingMiddleware");
        private const int MiddlewareEvenId = 70000;
        private readonly RequestDelegate _next;
        private readonly HttpContextLoggingMiddlewareOptions _options;
    }
}