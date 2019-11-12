using System.Collections.Generic;

namespace HappyTravel.StdOutLogger.Options
{
    public class HttpContextLoggingMiddlewareOptions
    {
        public HashSet<string> IgnoredPaths { get; set; } = new HashSet<string> {"/health"};
        public bool CollectRequestResponseLog { get; set; } = false;
    }
}