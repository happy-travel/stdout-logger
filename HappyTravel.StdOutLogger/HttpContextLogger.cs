using System;
using System.Threading.Tasks;
using HappyTravel.StdOutLogger.Internals;
using HappyTravel.StdOutLogger.Models;
using Microsoft.AspNetCore.Http;

namespace HappyTravel.StdOutLogger
{
    public class HttpContextLogger : IHttpContextLogger
    {
        public async Task AddHttpRequest(HttpRequest httpRequest)
        {
            _requestId = HttpLogHelper.GetRequestId(httpRequest);
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
        private string _requestId;
    }
}