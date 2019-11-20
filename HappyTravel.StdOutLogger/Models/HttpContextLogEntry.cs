using System;

namespace HappyTravel.StdOutLogger.Models
{
    public readonly struct HttpContextLogEntry
    {
        public HttpContextLogEntry(string requestId, DateTime createdAt, FormattedHttpRequest httpRequest = default,
            FormattedHttpResponse httpResponse = default)
        {
            CreatedAt = Equals(createdAt, default)
                ? string.Empty
                : createdAt.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffff");
            RequestId = requestId;
            HttpRequest = httpRequest;
            HttpResponse = httpResponse;
        }


        public string RequestId { get; }
        public string CreatedAt { get; }
        public FormattedHttpRequest HttpRequest { get; }
        public FormattedHttpResponse HttpResponse { get; }
    }
}