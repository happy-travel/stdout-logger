using System.Threading.Tasks;
using HappyTravel.StdOutLogger.Internals;
using HappyTravel.StdOutLogger.Models;
using HappyTravel.StdOutLogger.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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
            if (_options.IgnoredPaths.Contains(httpContext.Request.Path.Value.ToUpperInvariant()))
            {
                await _next(httpContext);
                return;
            }

            var formattedHttpRequest = await HttpLogHelper.GetFormattedHttpRequest(httpContext.Request);

            await _next(httpContext);

            var formattedHttpResponse = HttpLogHelper.GetFormattedHttpResponse(httpContext.Response);

            logger.Log(LogLevel.Information, _eventId, new HttpContextLogEntry(formattedHttpRequest, formattedHttpResponse), null,
                (entry, exception) => JsonConvert.SerializeObject(entry, JsonSerializerSettings));
        }


        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.None,
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore
        };


        private const int MiddlewareEvenId = 70000;

        private readonly EventId _eventId = new EventId(MiddlewareEvenId, "HttpLoggingMiddleware");
        private readonly RequestDelegate _next;
        private readonly HttpContextLoggingMiddlewareOptions _options;
    }
}