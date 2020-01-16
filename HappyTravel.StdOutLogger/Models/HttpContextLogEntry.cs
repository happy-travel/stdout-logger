using System;

namespace HappyTravel.StdOutLogger.Models
{
    internal readonly struct HttpContextLogEntry
    {
        public HttpContextLogEntry(FormattedHttpRequest httpRequest = default,
            FormattedHttpResponse httpResponse = default)
        {
            HttpRequest = httpRequest;
            HttpResponse = httpResponse;
        }
        
        
        public FormattedHttpRequest HttpRequest { get; }
        public FormattedHttpResponse HttpResponse { get; }
    }
}