using System;

namespace HappyTravel.StdOutLogger.Models
{
    internal readonly struct HttpContextLogEntry
    {
        public HttpContextLogEntry(DateTime createdAt, FormattedHttpRequest httpRequest = default,
            FormattedHttpResponse httpResponse = default)
        {
            CreatedAt = Equals(createdAt, default)
                ? string.Empty
                : createdAt.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffff");
            HttpRequest = httpRequest;
            HttpResponse = httpResponse;
        }
        

        public string CreatedAt { get; }
        public FormattedHttpRequest HttpRequest { get; }
        public FormattedHttpResponse HttpResponse { get; }
    }
}