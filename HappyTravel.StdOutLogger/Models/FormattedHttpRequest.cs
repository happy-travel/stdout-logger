using System.Collections.Generic;

namespace HappyTravel.StdOutLogger.Models
{
    public readonly struct FormattedHttpRequest
    {
        public FormattedHttpRequest(string traceId, string method, string host, string path, List<string> headers, string requestBody)
        {
            TraceId = traceId;
            Method = method;
            Host = host;
            Path = path;
            Headers = headers;
            RequestBody = requestBody;
        }


        public string TraceId { get; }
        public string Host { get; }
        public string Method { get; }
        public string Path { get; }
        public List<string> Headers { get; }
        public string RequestBody { get; }
    }
}