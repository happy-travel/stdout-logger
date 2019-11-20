using System.Globalization;
using System.Threading.Tasks;
using HappyTravel.StdOutLogger.Extensions;
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


        public async Task Invoke(HttpContext httpContext, IHttpContextLogger httpContextLogger,
            ILogger<HttpContextLoggingMiddleware> logger)
        {
            if (_options.IgnoredPaths.Contains(httpContext.Request.Path.Value.ToLower(CultureInfo.InvariantCulture)))
            {
                await _next(httpContext);
            }
            else
            {
                await httpContextLogger.AddHttpRequest(httpContext.Request);
                await _next(httpContext);

                if (_options.CollectRequestResponseLog)
                {
                    httpContextLogger.AddHttpResponse(httpContext.Response);
                    var httpContextLogModel = httpContextLogger.GetHttpContextLogModel();
                    logger.LogInformationToJson(default, string.Empty, httpContextLogModel);
                }
            }
        }


        private readonly RequestDelegate _next;
        private readonly HttpContextLoggingMiddlewareOptions _options;
    }
}