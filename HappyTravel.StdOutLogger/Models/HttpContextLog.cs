using System;

namespace HappyTravel.StdOutLogger.Models
{
    public readonly struct HttpContextLog
    {
        public HttpContextLog(string traceId, DateTime createdAt, FormattedHttpRequest httpRequest = default,
            FormattedHttpResponse httpResponse = default)
        {
            CreatedAt = Equals(createdAt, default)
                ? string.Empty
                : createdAt.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffff");
            TraceId = traceId;
            HttpRequest = httpRequest;
            HttpResponse = httpResponse;
        }


        public string TraceId { get; }
        public string CreatedAt { get; }
        public FormattedHttpRequest HttpRequest { get; }
        public FormattedHttpResponse HttpResponse { get; }
    }
}