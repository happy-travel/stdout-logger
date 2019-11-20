using System;
using System.Threading.Tasks;
using HappyTravel.StdOutLogger.Internals;
using HappyTravel.StdOutLogger.Models;
using HappyTravel.StdOutLogger.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace HappyTravel.StdOutLogger
{
    public class HttpContextLogger : IHttpContextLogger
    {
        public HttpContextLogger(IOptions<StdOutLoggerOptions> options)
        {
            _options = options.Value;
        }


        public async Task AddHttpRequest(HttpRequest httpRequest)
        {
            _requestId = HttpLogHelper.GetRequestId(httpRequest, _options.RequestIdHeader);
            _formattedHttpRequest = await HttpLogHelper.GetFormattedHttpRequest(httpRequest);
        }


        public void AddHttpResponse(HttpResponse httpResponse)
        {
            _formattedHttpResponse = HttpLogHelper.GetFormattedHttpResponse(httpResponse);
        }


        public HttpContextLogEntry GetHttpContextLogModel()
        {
            return new HttpContextLogEntry(_requestId, DateTime.UtcNow, _formattedHttpRequest, _formattedHttpResponse);
        }


        private FormattedHttpRequest _formattedHttpRequest;
        private FormattedHttpResponse _formattedHttpResponse;
        private readonly StdOutLoggerOptions _options;
        private string _requestId;
    }
}