using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
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


        public async Task Invoke(HttpContext httpContext, ILogger<HttpContextLoggingMiddleware> logger)
        {
            if (_options.IgnoredPaths.Contains(httpContext.Request.Path.Value?.ToUpperInvariant() ?? string.Empty))
            {
                await _next(httpContext);
                return;
            }

            var formattedHttpRequest = await HttpLogHelper.GetFormattedHttpRequest(httpContext.Request);

            await _next(httpContext);

            var formattedHttpResponse = HttpLogHelper.GetFormattedHttpResponse(httpContext.Response);

            logger.Log(LogLevel.Information, _eventId, new HttpContextLogEntry(formattedHttpRequest, formattedHttpResponse), null,
                (entry, _) => JsonSerializer.Serialize(entry, JsonSerializerOptions));
        }
        
        
        private static readonly JsonSerializerOptions JsonSerializerOptions = new()
        {
            WriteIndented = false,
            IgnoreNullValues = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
        };


        private const int MiddlewareEvenId = 70000;

        private readonly EventId _eventId = new (MiddlewareEvenId, "HttpLoggingMiddleware");
        private readonly RequestDelegate _next;
        private readonly HttpContextLoggingMiddlewareOptions _options;
    }
}